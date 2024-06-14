using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class SessionManager
{
    public event Action<Task<AuthenticationState>>? OnStateChanged;
    public event Action<SessionInfo?>? OnSessionChanged;
    
    const string TokenKey = "Jwt";

    readonly HttpService _http;
    readonly StorageService _storage;
    readonly Timer _timer;
    readonly object _fetchLock = new();
    
    bool _isLoading = false;

    string? _token;
    JwtSecurityToken? _jwt;
    AuthenticationState? _authState;
    SessionInfo? _session;

    readonly TimeSpan RefreshBufferTimespan = TimeSpan.FromSeconds( 15 );
    DateTime _lastUpdate = DateTime.Now;
    
    // CONSTRUCTOR
    public SessionManager( HttpService http, StorageService storage )
    {
        _http = http;
        _storage = storage;
        _timer = new Timer( _ => InvokeTimerCallback(), null, TimeSpan.Zero, TimeSpan.FromMinutes( 1 ) );

        async void InvokeTimerCallback()
        {
            if (!IsTokenExpired( _jwt ))
                return;

            Reply<string> fetchReply = await FetchFromServer();
            Logger.Log( fetchReply.IsOkay
                ? "Refreshed Jwt."
                : "Failed to refresh Jwt" );
        }
    }
    void InvokeNotify()
    {
        Task<AuthenticationState> stateParam = Task.FromResult( _authState ?? EmptyAuthenticationState() );
        OnStateChanged?.Invoke( stateParam );
        OnSessionChanged?.Invoke( _session );
    }
    
    // FOR AUTH PROVIDER CALLBACK
    internal async Task<AuthenticationState> GetSessionState()
    {
        Logger.Log( "Getting Authentication State..." );
        
        // WAIT FOR OTHER REQUESTS TO FINISH
        // DO NOT RE-FETCH IN NETWORK IF IT WAS JUST FETCHED
        //if (await WaitForOthers())
            //return _authState ?? EmptyAuthenticationState();

        //if (DateTime.Now - _lastUpdate > RefreshBufferTimespan)
            //return _authState ?? EmptyAuthenticationState();
        
        // IF ALREADY IN MEMORY
        //if (AuthStateValid( out AuthenticationState state ))
            //return state;
        
        StartLoading();
        await GetFromBrowserStorage();
        
        // IF ALREADY IN MEMORY
        //if (AuthStateValid( out var state ))
            //return state;
        
        await FetchFromServer();
        StopLoading();
        
        return AuthStateValid( out var state )
            ? state
            : EmptyAuthenticationState();
    }
    internal async Task<Reply<bool>> UpdateSession( string? accessToken )
    {
        Logger.Log( "Updating Session." );
        //await WaitForOthers();
        
        // EARLY OUT
        if (string.IsNullOrWhiteSpace( accessToken ))
        {
            Logger.LogError( "EMPTY TOKEN" );
            return await ClearSession();
        }
        
        StartLoading();
        
        lock ( _fetchLock )
        {
            _token = accessToken;
            _jwt = ParseJwtFromString( accessToken );
        }

        Logger.Log( $"Claims: {_jwt.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Name )}" );
        Reply<bool> accessResult = await _storage.Set( TokenKey, accessToken );
        await _storage.Set( TokenKey, accessToken );
        InvokeNotify();
        StopLoading();
        
        return accessResult.IsOkay
            ? IReply.True()
            : IReply.False( accessResult );
    }
    internal async Task<Reply<bool>> ClearSession()
    {
        Logger.Log( "Clearing Session." );
        await WaitForOthers();
        
        StartLoading();
        ParseAndSetData( null );
        Reply<bool> storageReply = await _storage.Remove( TokenKey );
        InvokeNotify();
        StopLoading();

        return storageReply.IsOkay
            ? IReply.True()
            : IReply.False( storageReply );
    }
    internal async Task<Reply<bool>> RefreshSession()
    {
        await WaitForOthers();
        
        StartLoading();
        Logger.Log( "Refreshing Authentication State..." );

        Reply<string> refreshReply = await FetchFromServer();
        
        Logger.Log( refreshReply.IsOkay
            ? "Authentication State Refreshed."
            : "Authentication Refresh Failed." );

        StopLoading();
        return refreshReply.IsOkay
            ? IReply.True()
            : IReply.False( refreshReply );
    }
    internal async Task<string?> AccessToken()
    {
        //if (!await WaitForOthers())
            //await GetSessionState();
        return _token;
    }
    internal string? Username()
    {
        return _jwt?.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name )?.ToString();
    }
    
    // COMMON
    async Task<bool> WaitForOthers()
    {
        bool hadToWait = false;
        int count = 0;
        while ( _isLoading && count < 1 )
        {
            hadToWait = true;
            await Task.Delay( 500 );
            count++;
        }
        return hadToWait;
    }
    async Task<Reply<string>> FetchFromServer()
    {
        Reply<string> refreshReply = await _http.PostAsync<string>( Consts.ApiLoginRefresh );
        string? token = refreshReply.IsOkay
            ? refreshReply.Data
            : null;

        ParseAndSetData( token );
        InvokeNotify();
        return refreshReply;
    }
    async Task GetFromBrowserStorage()
    {
        if (!string.IsNullOrWhiteSpace( _token ) && _jwt is not null)
            return;

        lock ( _fetchLock )
            _jwt = null;

        Reply<string> tokenReply = await _storage.GetString( TokenKey );
        Logger.Log( $"KEY FROM STORAGE {tokenReply.Data}" );
        if (!tokenReply.IsOkay)
            return;

        lock ( _fetchLock )
        {
            _token = tokenReply.Data;
            _jwt = ParseJwtFromString( _token );
        }
        
        Logger.Log( _jwt?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).ToString() );
    }
    bool AuthStateValid( out AuthenticationState state )
    {
        state = EmptyAuthenticationState();
        if (_authState is null || IsTokenExpired( _jwt ))
            return false;
        state = _authState;
        return true;
    }
    void ParseAndSetData( string? token )
    {
        lock ( _fetchLock )
        {
            _lastUpdate = DateTime.Now;
            
            if (string.IsNullOrWhiteSpace( token ))
            {
                _token = null;
                _jwt = null;
                _authState = null;
                _session = null;
            }
            else
            {
                _token = token;
                _jwt = ParseJwtFromString( _token );
                _authState = ParseStateFromJwt( _jwt );
                _session = ParseSessionFromJwt( _jwt );
            }
        }
    }
    void StartLoading()
    {
        lock ( _fetchLock )
            _isLoading = true;
    }
    void StopLoading()
    {
        lock ( _fetchLock )
            _isLoading = false;
    }
    static JwtSecurityToken? ParseJwtFromString( string str )
    {
        if (string.IsNullOrWhiteSpace( str ))
            return null;

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken( str );
        return token;
    }
    static AuthenticationState ParseStateFromJwt( JwtSecurityToken? token )
    {
        if (token is null)
            return EmptyAuthenticationState();

        Claim? nameIdentifierClaim = token.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.NameIdentifier );
        Claim? nameClaim = token.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name );

        ClaimsIdentity identity = new( new[] {
            nameIdentifierClaim ?? new Claim( ClaimTypes.NameIdentifier, "None" ),
            nameClaim ?? new Claim( ClaimTypes.Name, "None" )
        }, "token" );

        return new AuthenticationState( new ClaimsPrincipal( identity ) );
    }
    static SessionInfo? ParseSessionFromJwt( JwtSecurityToken? jwt )
    {
        if (jwt is null)
            return null;

        string id = jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.NameIdentifier )?.Value ?? "Account Id";
        string name = jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name )?.Value ?? "User Name";

        return new SessionInfo( id, name );
    }
    static AuthenticationState EmptyAuthenticationState() =>
        new( new ClaimsPrincipal() );
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
            
            if (timeDifference.TotalMinutes > 5)
                Logger.LogError( "Token is expired." );

            return timeDifference.TotalMinutes <= 5;
        }
        catch ( Exception e )
        {
            Logger.LogError( e, "An exception occured while checking if a jwt token is expired." );
            return true;
        }
    }
}