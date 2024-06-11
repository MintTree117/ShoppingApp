using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class AuthenticationService
{
    const string TokenKey = "Jwt";

    readonly HttpService _http;
    readonly StorageService _storage;
    readonly TimeSpan _refreshTimespan = TimeSpan.FromMinutes( 3 );
    readonly Timer _timer;
    readonly object _fetchLock = new();
    
    bool _isFetching = false;
    bool _isFirstFetch = true;

    string? _token;
    JwtSecurityToken? _jwt;

    public AuthenticationService( HttpService http, StorageService storage )
    {
        _http = http;
        _storage = storage;
        _timer = new Timer( _ => InvokeTimerCallback(), null, TimeSpan.Zero, TimeSpan.FromMinutes( 1 ) );
    }
    public event Action<Task<AuthenticationState>>? OnStateChanged;
    async void InvokeTimerCallback()
    {
        if (!IsTokenExpired( _jwt ))
            return;
        
        Reply<string> fetchReply = await FetchRefresh();
        Logger.Log( fetchReply.IsOkay
            ? "Refreshed Jwt."
            : "Failed to refresh Jwt" );
    }
    void InvokeNotifyStateChanged()
    {
        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( _jwt );
        Task<AuthenticationState> param = Task.FromResult( new AuthenticationState( claims ) );
        OnStateChanged?.Invoke( param );
    }
    
    internal async Task<AuthenticationState> GetState()
    {
        // WAIT FOR OTHER REQUESTS TO FINISH
        bool waiting = false;
        while ( _isFetching ) 
        {
            waiting = true;
            await Task.Delay( 500 );
            Logger.Log( "AUTH SERVICE: Awaiting another fetch." );
        }
        
        // DO NOT RE-FETCH IN NETWORK IF IT WAS JUST FETCHED
        if (waiting)
            return _jwt is not null
                ? new AuthenticationState( GetIdentityClaimsPrincipal( _jwt ) )
                : new AuthenticationState( new ClaimsPrincipal() );
        
        // START FETCHING
        lock ( _fetchLock )
            _isFetching = true;
        
        // GET LOCAL
        await PopulateLocal();
        
        // CHECK TOKEN
        if (_isFirstFetch || _jwt is null || IsTokenExpired( _jwt ))
        {
            Reply<string> refreshReply = await FetchRefresh();

            lock ( _fetchLock )
            {
                _isFirstFetch = false;
                _isFetching = false;   
            }
            
            return refreshReply.IsOkay
                ? new AuthenticationState( GetIdentityClaimsPrincipal( _jwt ) )
                : new AuthenticationState( new ClaimsPrincipal() );
        }
        
        // RETURN VALID IN-MEMORY
        lock ( _fetchLock )
            _isFetching = false;
        
        return new AuthenticationState( GetIdentityClaimsPrincipal( _jwt ) );
    }
    internal async Task<Reply<bool>> SetState( string? token )
    {
        if (string.IsNullOrWhiteSpace( token ))
        {
            Logger.LogError( "AuthService SetState(); Invalid access token" );
            return IReply.False( "Invalid access token." );
        }

        lock ( _fetchLock )
        {
            _token = token;
            _jwt = string.IsNullOrWhiteSpace( token ) ? null : ParseJwtFromString( token );
        }
        
        Reply<bool> accessResult = await _storage.Set( TokenKey, token );
        InvokeNotifyStateChanged();

        return accessResult.IsOkay
            ? IReply.True()
            : IReply.False( accessResult );
    }
    internal async Task<Reply<bool>> ClearState()
    {
        lock ( _fetchLock )
        {
            _token = null;
            _jwt = null;
        }
        
        Reply<bool> storageReply = await _storage.Remove( TokenKey );
        InvokeNotifyStateChanged();
        
        return storageReply.IsOkay
            ? IReply.True()
            : IReply.False( storageReply );
    }
    internal async Task<string?> AccessToken()
    {
        AuthenticationState state = await GetState();
        return _token;
    }
    /*internal async Task<Reply<string>> GetAccessToken()
    {
        AuthenticationState state = await GetState();

        return string.IsNullOrWhiteSpace( _token )
            ? Reply<string>.False( "Failed to get access token" )
            : Reply<string>.True( _token );
    }*/
    internal async Task<Reply<bool>> RefreshSessionInfo()
    {
        Logger.Log( "Refreshing Authentication State..." );

        await PopulateLocal();

        /*Reply<string> refreshReply = await FetchRefresh();
        InvokeNotify();

        Logger.Log( refreshReply.IsOkay
            ? "Authentication State Refreshed."
            : "Authentication Refresh Failed." );

        return refreshReply.IsOkay
            ? IReply.True()
            : IReply.False( refreshReply );*/
        return IReply.False( "" );
    }

    async Task<Reply<string>> FetchRefresh()
    {
        Reply<string> refreshReply = await _http.GetAsyncAuthenticated<string>( Consts.ApiLoginRefresh );

        if (refreshReply.IsOkay)
        {
            lock ( _fetchLock )
            {
                _token = refreshReply.Data;
                _jwt = ParseJwtFromString( _token );
            }
            InvokeNotifyStateChanged();
            return refreshReply;
        }

        lock ( _fetchLock )
        {
            _token = string.Empty;
            _jwt = null;
        }
        
        InvokeNotifyStateChanged();
        return refreshReply;
    }
    async Task PopulateLocal()
    {
        if (!string.IsNullOrWhiteSpace( _token ) && _jwt is not null )
            return;

        lock ( _fetchLock )
            _jwt = null;
        
        Reply<string> tokenReply = await _storage.GetString( TokenKey );
        if (!tokenReply.IsOkay)
            return;

        lock ( _fetchLock )
        {
            _token = tokenReply.Data;
            _jwt = ParseJwtFromString( _token );
        }
    }
    static JwtSecurityToken? ParseJwtFromString( string str )
    {
        if (string.IsNullOrWhiteSpace( str ))
            return null;

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken( str );
        return token;
    }
    static ClaimsPrincipal GetIdentityClaimsPrincipal( JwtSecurityToken? token )
    {
        if (token is null)
            return new ClaimsPrincipal();

        Claim? nameIdentifierClaim = token.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.NameIdentifier );
        Claim? nameClaim = token.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name );

        ClaimsIdentity identity = new( new[] {
            nameIdentifierClaim ?? new Claim( ClaimTypes.NameIdentifier, "None" ),
            nameClaim ?? new Claim( ClaimTypes.Name, "None" )
        }, "token" );

        return new ClaimsPrincipal( identity );
    }
    static bool IsTokenExpired( JwtSecurityToken? token )
    {
        try
        {
            if (token is null)
                return true;
            
            long expiryDateUnix = long.Parse( token.Claims.First( static claim => claim.Type == "exp" ).Value );
            DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds( expiryDateUnix ).UtcDateTime;

            DateTime currentTime = DateTime.UtcNow;
            TimeSpan timeDifference = expiryDateTime - currentTime;

            return timeDifference.TotalMinutes <= 5;
        }
        catch ( Exception e )
        {
            Logger.LogError( e, "An exception occured while checking if a jwt token is expired." );
            return true;
        }
    }
}