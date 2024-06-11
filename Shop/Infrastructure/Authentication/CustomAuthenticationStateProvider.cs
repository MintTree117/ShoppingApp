using Microsoft.AspNetCore.Components.Authorization;

namespace Shop.Infrastructure.Authentication;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable // SCOPED
{
    readonly AuthenticationService _authenticationService;
    
    public CustomAuthenticationStateProvider( AuthenticationService authenticationService )
    {
        _authenticationService = authenticationService;
        _authenticationService.OnStateChanged += NotifyAuthenticationStateChanged;
    }
    public void Dispose()
    {
        _authenticationService.OnStateChanged -= NotifyAuthenticationStateChanged;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        AuthenticationState result = await _authenticationService.GetState();
        return result;
    }
}