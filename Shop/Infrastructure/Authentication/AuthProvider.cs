using Microsoft.AspNetCore.Components.Authorization;

namespace Shop.Infrastructure.Authentication;

public sealed class AuthProvider : AuthenticationStateProvider, IDisposable // SCOPED
{
    readonly AuthService _authService;
    
    public AuthProvider( AuthService authService )
    {
        _authService = authService;
        _authService.OnStateChanged += Notify;
    }
    public void Dispose()
    {
        _authService.OnStateChanged -= Notify;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("----------------------------GETTING");
        AuthenticationState result = await _authService.GetState();
        return result;
    }

    void Notify( Task<AuthenticationState> task )
    {
        NotifyAuthenticationStateChanged( task );
    }
}