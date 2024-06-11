using Microsoft.AspNetCore.Components.Authorization;

namespace Shop.Infrastructure.Authentication;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable // SCOPED
{
    readonly SessionManager _sessionManager;
    
    public CustomAuthenticationStateProvider( SessionManager sessionManager )
    {
        _sessionManager = sessionManager;
        _sessionManager.OnStateChanged += NotifyAuthenticationStateChanged;
    }
    public void Dispose()
    {
        _sessionManager.OnStateChanged -= NotifyAuthenticationStateChanged;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        AuthenticationState result = await _sessionManager.GetSessionState();
        return result;
    }
}