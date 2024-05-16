using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Common.Optionals;

namespace ShopApplication.Features.Identity;

public interface IIdentityManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<Val<bool>> UpdateAuthenticationState( string? token );
    public Task<Val<bool>> ClearAuthenticationState();
}