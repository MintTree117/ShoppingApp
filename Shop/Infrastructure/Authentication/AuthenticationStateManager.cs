using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Types.Common.ReplyTypes;
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
    DateTime _nextExpiryUtc = DateTime.UtcNow - TimeSpan.FromDays( 9 );
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
        
        // IN-MEMORY
        SetBusy( true );
        if (IsSessionValid())
        {
            InvokeNotify();
            Logger.Log( "Session manager returning valid in-memory session." );
            return await Task.FromResult( _authenticationState );
        }
        
        // STORAGE OR SERVER
        await GetTokenFromBrowserStorage();
        if (!IsSessionValid()) // TODO: Test if we need to clear memory here
            await GetTokenFromServer();
        
        SetBusy( false );
        InvokeNotify();
        Logger.Log( IsSessionValid()
            ? "Session manager returning valid in-memory session."
            : "Session manager did not find a valid session on request." );
        return await Task.FromResult( _authenticationState );
    }
    internal async Task<Reply<bool>> CreateNewSession( string newToken )
    {
        await SessionIsAlreadyBeingRefreshed();
        SetBusy( true );
        SetMemory( newToken );
        Logger.Log( await _storage.SetLocalStorageString( SessionStorageTokenKey, newToken )
            ? "Session manager saved new session to local storage"
            : $"Session manager failed to save new session to local storage." );
        SetBusy( false );
        InvokeNotify();
        return IReply.Success();
    }
    internal async Task ClearSession()
    {
        await SessionIsAlreadyBeingRefreshed();
        SetBusy( true );
        ClearMemory();
        Logger.Log( await _storage.RemoveLocalStorage( SessionStorageTokenKey )
            ? "Session manager cleared session state from storage."
            : "Session manager failed to clear token from storage." );
        SetBusy( false );
        InvokeNotify();
    }
    internal async Task ForceRefresh()
    {
        await SessionIsAlreadyBeingRefreshed();
        SetBusy( true );
        ClearMemory();
        Logger.Log( await GetTokenFromServer()
            ? "Session manager refreshed session."
            : "Session manager failed to refresh session." );
        SetBusy( false );
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
        var tokenReply = await _storage.GetLocalStorageString( SessionStorageTokenKey );
        if (!tokenReply)
            return;

        Logger.Log( $"Token from storage: {tokenReply.Data}" );
        SetMemory( tokenReply.Data );
    }
    async Task<bool> GetTokenFromServer()
    {
        var serverReply = await _http.PostAsync<string>( Consts.ApiLoginRefresh );
        if (!serverReply)
        {
            Logger.Log( $"Session manager failed to fetch from server. {serverReply.GetMessage()}" );
            return false;
        }

        SetMemory( serverReply.Data );
        await _storage.SetLocalStorageString( SessionStorageTokenKey, _token ?? string.Empty );
        return true;
    }

    bool IsSessionValid() =>
        !string.IsNullOrWhiteSpace( _token ) &&
        DateTime.UtcNow < _nextExpiryUtc;
    void SetMemory( string token )
    {
        _token = token;
        
        JwtSecurityToken? _jwt = ParseJwtFromString( _token );
        ClaimsIdentity identity = new( _jwt!.Claims, "jwtAuthType" );
        ClaimsPrincipal _claimsPrincipal = new( identity );
        _authenticationState = new AuthenticationState( _claimsPrincipal );

        /*
        string sessionId = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Sid )?.Value ?? "SessionId Empty";
        string userId = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.NameIdentifier )?.Value ?? "UserId Empty";
        string username = _jwt.Claims.FirstOrDefault( static c => c.Type == ClaimTypes.Name )?.Value ?? "Username Empty";
        */
        
        long expiryDateUnix = long.Parse( _jwt.Claims.FirstOrDefault( static claim => claim.Type == "exp" )?.Value ?? string.Empty );
        DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds( expiryDateUnix ).UtcDateTime;
        _nextExpiryUtc = expiryDateTime;
    }
    
    void ClearMemory()
    {
        lock ( _fetchLock )
        {
            _token = null;
            _nextExpiryUtc = DateTime.Now - TimeSpan.FromDays( 9 );
            _authenticationState = EmptyAuthenticationState();
        }
    }
    void SetBusy( bool value )
    {
        lock ( _fetchLock )
            _isLoading = value;
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