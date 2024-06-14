using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class SessionManager
{
    public event Action<Task<AuthenticationState>>? OnStateChanged;
    public event Action<SessionClaims?>? OnSessionChanged;
    
    const string TokenKey = "Jwt";

    readonly HttpService _http;
    readonly StorageService _storage;
    readonly Timer _timer;
    readonly object _fetchLock = new();
    readonly TimeSpan RefreshBufferTimespan = TimeSpan.FromSeconds( 60 );
    
    bool _isLoading = false;
    bool _isFirstLoad = true; // on startup go to server
    
    internal string? AccessToken => _token ?? string.Empty;
    string? _token;
    SessionClaims? _session;
    AuthenticationState _authenticationState = EmptyAuthenticationState();
    
    // CONSTRUCTOR
    public SessionManager( HttpService http, StorageService storage )
    {
        _http = http;
        _storage = storage;
        _timer = new Timer( _ => InvokeTimerCallback(), null, TimeSpan.FromSeconds( 20 ), TimeSpan.FromMinutes( 1 ) );

        async void InvokeTimerCallback()
        {
            if (IsSessionValid())
                return;

            var fetchReply = await GetTokenFromServer();
            Logger.Log( fetchReply
                ? "Refreshed Session."
                : "Failed to refresh Session." );
        }
    }
    void InvokeNotify()
    {
        Task<AuthenticationState> stateParam = Task.FromResult( _authenticationState );
        OnStateChanged?.Invoke( stateParam );
        OnSessionChanged?.Invoke( _session );
    }
    
    // FOR AUTH PROVIDER CALLBACK
    internal async Task<AuthenticationState> GetSessionState()
    {
        Logger.Log( "Session manager is getting session state." );
        
        // On startup, always go to server once
        if (_isFirstLoad)
        {
            _isFirstLoad = false;

            if (!await GetTokenFromServer())
                Logger.Log( "Session manager is unauthenticated on first load." );
            
            InvokeNotify(); // because its the first fetch
            return await Task.FromResult( _authenticationState );
        }

        // Someone else is updating
        if (await StateIsBeingRefreshed())
            return await Task.FromResult( _authenticationState );
        
        // Valid in-memory
        if (IsSessionValid())
            return await Task.FromResult( _authenticationState );
        
        // Fetch
        StartLoading();
        await GetTokenFromBrowserStorage();
        if (!IsSessionValid())
            await GetTokenFromServer();
        StopLoading();

        return await Task.FromResult( _authenticationState );
    }
    internal async Task<Reply<bool>> CreateNewSession( string newToken )
    {
        Logger.Log( "Session manager creating new session." );
        await StateIsBeingRefreshed();
        
        StartLoading();
        SetMemory( newToken );
        await SetStorage( newToken );
        StopLoading();

        InvokeNotify();
        return IReply.Success();
    }
    internal async Task<Reply<bool>> ClearSession()
    {
        Logger.Log( "Session manager clearing session." );
        await StateIsBeingRefreshed();

        StartLoading();
        ClearMemory();
        var storageReply = await _storage.Remove( TokenKey );
        StopLoading();

        InvokeNotify();
        return IReply.Success();
    }
    internal async Task<Reply<bool>> ForceRefresh()
    {
        Logger.Log( "Session manager force refreshing." );
        await StateIsBeingRefreshed();

        StartLoading();
        await GetTokenFromServer();
        StopLoading();
        
        InvokeNotify();
        return IReply.Success();
    }
    
    async Task<bool> StateIsBeingRefreshed()
    {
        Logger.Log( "Session manager, client is waiting." );
        bool hadToWait = false;
        int count = 0;
        while ( _isLoading && count < 2 )
        {
            hadToWait = true;
            await Task.Delay( 500 );
            count++;
        }
        return hadToWait;
    }
    async Task<bool> GetTokenFromBrowserStorage()
    {
        ClearMemory();
        
        var tokenReply = await _storage.GetString( TokenKey );
        Logger.Log( $"Session manager, token from browser storage = {tokenReply.Data}" );
        if (!tokenReply)
            return false;
        
        SetMemory( tokenReply.Data );
        return true;
    }
    async Task<bool> GetTokenFromServer()
    {
        Logger.Log( "Session manager is fetching from server." );
        var refreshReply = await _http.PostAsync<string>( Consts.ApiLoginRefresh );
        if (!refreshReply)
            return false;

        SetMemory( refreshReply.Data );
        await SetStorage( refreshReply.Data );
        return true;
    }
    async Task<bool> SetStorage( string token )
    {
        var reply = await _storage.Set( TokenKey, token );
        return reply;
    }
    
    void SetMemory( string token )
    {
        _token = token;
        
        JwtSecurityToken? _jwt = ParseJwtFromString( _token );
        ClaimsIdentity identity = new( _jwt!.Claims, "jwtAuthType" );
        ClaimsPrincipal _claimsPrincipal = new( identity );
        _authenticationState = new AuthenticationState( _claimsPrincipal );

        string sessionId = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Sid )?.Value ?? "SessionId Empty";
        string userId = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.NameIdentifier )?.Value ?? "UserId Empty";
        string username = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name )?.Value ?? "Username Empty";
        long expiryDateUnix = long.Parse( _jwt.Claims.FirstOrDefault( static claim => claim.Type == "exp" )?.Value ?? string.Empty );
        DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds( expiryDateUnix ).UtcDateTime;
        _session = new SessionClaims(
            sessionId,
            userId,
            username,
            expiryDateTime );
    }
    void ClearMemory()
    {
        lock ( _fetchLock )
        {
            _token = null;
            _session = null;
            _authenticationState = EmptyAuthenticationState();
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
    bool IsSessionValid() =>
        !string.IsNullOrWhiteSpace( _token ) &&
        _session is not null &&
        DateTime.Now < _session.Value.Expiry;
    
    static JwtSecurityToken? ParseJwtFromString( string str )
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken( str );
        return token;
    }
    static AuthenticationState EmptyAuthenticationState() =>
        new( new ClaimsPrincipal() );
}