using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Identity;

public interface IIdentityManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<Val<bool>> UpdateAuthenticationState( string token );
    public Task<Val<bool>> ClearAuthenticationState();
}