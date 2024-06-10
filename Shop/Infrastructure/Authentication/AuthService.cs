using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Shop.Infrastructure.Authentication.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class AuthService // Singleton
{
    const string AccessKey = "AccessToken";
    const string RefreshKey = "RefreshToken";

    readonly JwtSettings _rules;
    readonly HttpService _http;
    readonly StorageService _storage;
    readonly object _fetchLock = new();
    
    bool _isFetching = false; // Singleton
    bool _isFirstFetch = true;

    string _accessString = string.Empty;
    string _refreshString = string.Empty;
    JwtSecurityToken? _accessJwt;
    
    public event Action<Task<AuthenticationState>>? OnStateChanged;
    public AuthService( IConfiguration config, HttpService http, StorageService storage )
    {
        _rules = GetJwtRules( config );
        _http = http;
        _storage = storage;
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
            return _accessJwt is not null
                ? new AuthenticationState( GetIdentityClaimsPrincipal( _accessJwt ) )
                : new AuthenticationState( new ClaimsPrincipal() );
        
        // START FETCHING
        lock ( _fetchLock )
            _isFetching = true;
        
        // GET LOCAL
        await FetchFromStorage();
        
        // CHECK TOKEN
        if (_isFirstFetch || _accessJwt is null || TokenExpired( _accessJwt ))
        {
            //if (TokenExpired( _accessJwt ))
                //Logger.LogError( "EXPIREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE" );
            
            Reply<RefreshReply> refreshReply = await FetchRefresh();

            lock ( _fetchLock )
            {
                _isFirstFetch = false;
                _isFetching = false;   
            }
            
            return refreshReply.IsOkay
                ? new AuthenticationState( GetIdentityClaimsPrincipal( _accessJwt ) )
                : new AuthenticationState( new ClaimsPrincipal() );
        }
        
        // RETURN VALID IN-MEMORY
        lock ( _fetchLock )
            _isFetching = false;
        return new AuthenticationState( GetIdentityClaimsPrincipal( _accessJwt ) );
    }
    internal async Task<Reply<bool>> SetState( AuthRefreshEventArgs args )
    {
        if (string.IsNullOrWhiteSpace( args.AccessToken ))
        {
            Logger.LogError( "Invalid access token" );
            return IReply.None( "Invalid access token" );
        }
        
        if (string.IsNullOrWhiteSpace( args.RefreshToken ))
        {
            Logger.LogError( "Invalid refresh token" );
            return IReply.None( "Invalid refresh token" );
        }


        lock ( _fetchLock )
        {
            _accessString = args.AccessToken;
            _refreshString = args.RefreshToken;
            _accessJwt = ParseJwtFromString( _accessString );
        }
        
        Reply<bool> accessResult = await _storage.Set( AccessKey, _accessString );
        Reply<bool> refreshResult = await _storage.Set( RefreshKey, _refreshString );
        
        InvokeNotify();

        return accessResult.IsOkay && refreshResult.IsOkay
            ? IReply.Okay()
            : IReply.None( "Failed to save authentication to local storage." );
    }
    internal async Task<Reply<bool>> RefreshState()
    {
        Logger.Log( "AUTH SERVICE: Refreshing" );
        
        Reply<string> aTokenResult = await _storage.Get<string>( AccessKey );
        Reply<string> rTokenResult = await _storage.Get<string>( RefreshKey );
        
        if (!aTokenResult.IsOkay && !rTokenResult.IsOkay)
            return IReply.None( "No token data found in local storage." );

        Reply<RefreshReply> refreshReply = await FetchRefresh();
        InvokeNotify();
        
        return refreshReply.IsOkay
            ? IReply.Okay()
            : IReply.None( refreshReply );
    }
    internal async Task<Reply<bool>> ClearState()
    {
        _accessString = string.Empty;
        _refreshString = string.Empty;
        _accessJwt = null;
        
        Reply<bool> result1 = await _storage.Remove( AccessKey );
        Reply<bool> result2 = await _storage.Remove( RefreshKey );
        
        InvokeNotify();
        
        return result1.IsOkay && result2.IsOkay
            ? IReply.Okay()
            : IReply.None( "Failed to clear authentication state properly." );
    }
    internal async Task<Reply<string>> GetAccessToken()
    {
        if (!string.IsNullOrWhiteSpace( _accessString ))
            return Reply<string>.With( _accessString );
        Reply<string> storageReply = await _storage.Get<string>( AccessKey );
        if (storageReply.IsOkay)
            _accessString = storageReply.Data;
        return storageReply;
    }

    async Task<Reply<RefreshReply>> FetchRefresh()
    {
        RefreshRequest request = new( _accessString, _refreshString );
        Reply<RefreshReply> refreshReply = await _http.TryPostRequest<RefreshReply>( Consts.ApiLoginRefresh, request );
        
        if (refreshReply.IsOkay)
        {
            lock ( _fetchLock )
            {
                _accessString = refreshReply.Data.AccessToken ?? string.Empty;
                _accessJwt = ParseJwtFromString( _accessString );
            }
            return refreshReply;
        }

        lock ( _fetchLock )
        {
            _accessString = string.Empty;
            _refreshString = string.Empty;
            _accessJwt = null;
        }
        
        return refreshReply;
    }
    async Task FetchFromStorage()
    {
        _accessString = string.Empty;
        _refreshString = string.Empty;
        
        if (string.IsNullOrWhiteSpace( _accessString ))
        {
            Reply<string> accessReply = await _storage.Get<string>( AccessKey );

            if (accessReply.IsOkay)
                _accessString = accessReply.Data;
            else
                Logger.Log( "No access token found in local storage." );
        }

        if (string.IsNullOrWhiteSpace( _refreshString ))
        {
            Reply<string> refreshReply = await _storage.Get<string>( RefreshKey );
            if (refreshReply.IsOkay)
                _refreshString = refreshReply.Data;
            else
                Logger.Log( "No refresh token found in local storage." );
        }

        _accessJwt = ParseJwtFromString( _accessString );
    }

    static bool TokenExpired( JwtSecurityToken token )
    {
        try {
            long expiryDateUnix = long.Parse( token.Claims.First( static claim => claim.Type == "exp" ).Value );
            DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds( expiryDateUnix ).UtcDateTime;

            DateTime currentTime = DateTime.UtcNow;
            TimeSpan timeDifference = expiryDateTime - currentTime;

            return timeDifference.TotalMinutes <= 5;
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return true;
        }
    }
    static JwtSettings GetJwtRules( IConfiguration config ) =>
        new() {
            Key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( config.GetOrThrow( "Identity:Jwt:Key" ) ) ),
            Audience = config.GetOrThrow( "Identity:Jwt:Audience" ),
            Issuer = config.GetOrThrow( "Identity:Jwt:Issuer" ),
            ValidateAudience = config.GetSection( "Identity:Jwt:ValidateAudience" ).Get<bool>(),
            ValidateIssuer = config.GetSection( "Identity:Jwt:ValidateIssuer" ).Get<bool>(),
            ValidateIssuerSigningKey = config.GetSection( "Identity:Jwt:ValidateIssuerSigningKey" ).Get<bool>(),
            AccessLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:AccessLifetime" ) ),
            RefreshLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:RefreshLifetime" ) )
        };
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
        Claim? nameIdentifierClaim = token?.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier );
        Claim? nameClaim = token?.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Name );

        ClaimsIdentity identity = new( new[] {
            nameIdentifierClaim ?? new Claim( ClaimTypes.NameIdentifier, "None" ),
            nameClaim ?? new Claim( ClaimTypes.Name, "None" )
        }, "token" );
        
        return new ClaimsPrincipal( identity );
    }

    void InvokeNotify()
    {
        ClaimsPrincipal? claims = GetIdentityClaimsPrincipal( _accessJwt );
        Task<AuthenticationState> param = Task.FromResult( new AuthenticationState( claims ?? new ClaimsPrincipal() ) );
        OnStateChanged?.Invoke( param );
    }
    
    readonly record struct RefreshRequest(
        string? AccessToken,
        string RefreshToken );
    readonly record struct RefreshReply(
        string? AccessToken );
}