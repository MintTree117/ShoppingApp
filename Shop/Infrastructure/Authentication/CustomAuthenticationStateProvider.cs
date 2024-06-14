using Microsoft.AspNetCore.Components.Authorization;

namespace Shop.Infrastructure.Authentication;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable // SCOPED
{
    readonly AuthenticationStateManager _authenticationStateManager;
    
    public CustomAuthenticationStateProvider( AuthenticationStateManager authenticationStateManager )
    {
        _authenticationStateManager = authenticationStateManager;
        _authenticationStateManager.OnStateChanged += NotifyAuthenticationStateChanged;
    }
    public void Dispose()
    {
        _authenticationStateManager.OnStateChanged -= NotifyAuthenticationStateChanged;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        AuthenticationState result = await _authenticationStateManager.GetSessionState();
        return result;
    }
}