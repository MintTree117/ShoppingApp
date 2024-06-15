using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class AuthenticationStateManager
{
    public event Action<Task<AuthenticationState>>? OnStateChanged;
    
    const string SessionStorageTokenKey = "Jwt";

    readonly HttpService _http;
    readonly StorageService _storage;
    readonly Timer _timer;
    readonly object _fetchLock = new();
    
    bool _isLoading = false;

    internal string AccessToken => _token ?? string.Empty;
    string? _token = null;
    DateTime _nextExpiry = DateTime.Now - TimeSpan.FromDays( 9 );
    AuthenticationState _authenticationState = EmptyAuthenticationState();
    
    // CONSTRUCTOR
    public AuthenticationStateManager( HttpService http, StorageService storage )
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
                ? "Session manager auto-refreshed session successfully."
                : "Session manager failed to auto-refresh session successfully" );
        }
    }
    void InvokeNotify()
    {
        Task<AuthenticationState> stateParam = Task.FromResult( _authenticationState );
        OnStateChanged?.Invoke( stateParam );
    }
    
    // FOR AUTH PROVIDER CALLBACK
    internal async Task<AuthenticationState> GetSessionState()
    {
        if (await SessionIsAlreadyBeingRefreshed())
            return await Task.FromResult( _authenticationState );
        
        StartLoading();
        
        Logger.Log( "Session manager getting session state from memory" );
        if (IsSessionValid())
        {
            InvokeNotify();
            return await Task.FromResult( _authenticationState );
        }


        Logger.Log( "Session manager getting session state from storage" );
        await GetTokenFromBrowserStorage();
        
        if (!IsSessionValid())
        {
            Logger.Log( "Session manager getting session state from server" );
            await GetTokenFromServer();
        }
        StopLoading();
        InvokeNotify();
        return await Task.FromResult( _authenticationState );
    }
    internal async Task<Reply<bool>> CreateNewSession( string newToken )
    {
        await SessionIsAlreadyBeingRefreshed();
        Logger.Log( "Session manager creating new session." );        
        StartLoading();
        SetMemory( newToken );
        await _storage.Set( SessionStorageTokenKey, newToken );
        StopLoading();
        InvokeNotify();
        return IReply.Success();
    }
    internal async Task ClearSession()
    {
        await SessionIsAlreadyBeingRefreshed();
        Logger.Log( "Session manager clearing session." );
        StartLoading();
        ClearMemory();
        var reply = await _storage.Remove( SessionStorageTokenKey );
        Logger.Log( reply ? "Session manager cleared session state from storage." : $"Session manager failed to clear token from storage." );
        StopLoading();
        InvokeNotify();
    }
    internal async Task ForceRefresh()
    {
        await SessionIsAlreadyBeingRefreshed();
        Logger.Log( "Session manager force refreshing." );
        StartLoading();
        ClearMemory();
        var reply = await GetTokenFromServer();
        Logger.Log( reply ? "Session manager refreshed session." : "Session manager failed to refresh session." );
        StopLoading();
        InvokeNotify();
    }
    
    async Task<bool> SessionIsAlreadyBeingRefreshed()
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
    async Task GetTokenFromBrowserStorage()
    {
        ClearMemory();
        var tokenReply = await _storage.GetString( SessionStorageTokenKey );
        if (!tokenReply)
            return;

        Logger.Log( $"Token from storage: {tokenReply.Data}" );
        SetMemory( tokenReply.Data );
    }
    async Task<bool> GetTokenFromServer()
    {
        Logger.Log( "Session manager is fetching from server." );
        Reply<string> refreshReply = await _http.PostAsync<string>( Consts.ApiLoginRefresh );
        if (!refreshReply)
        {
            Logger.Log( "Session manager failed to fetch from server" );
            return false;
        }

        SetMemory( refreshReply.Data );
        await _storage.Set( SessionStorageTokenKey, _token );
        return true;
    }

    bool IsSessionValid() =>
        !string.IsNullOrWhiteSpace( _token ) &&
        DateTime.Now < _nextExpiry;
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
        _nextExpiry = expiryDateTime;
    }
    void ClearMemory()
    {
        lock ( _fetchLock )
        {
            _token = null;
            _nextExpiry = DateTime.Now - TimeSpan.FromDays( 9 );
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
    
    static JwtSecurityToken? ParseJwtFromString( string str )
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken( str );
        return token;
    }
    static AuthenticationState EmptyAuthenticationState() =>
        new( new ClaimsPrincipal() );
}