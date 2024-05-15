using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Identity;

public interface IIdentityManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<OptVal<bool>> UpdateAuthenticationState( string token );
    public Task<OptVal<bool>> ClearAuthenticationState();
}