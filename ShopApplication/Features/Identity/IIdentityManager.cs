using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Common.Optionals;

namespace ShopApplication.Features.Identity;

public interface IIdentityManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<Val<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken );
    public Task<Val<bool>> RefreshAuthenticationStateAsync( string? accessToken );
    public Task<Val<bool>> ClearAuthenticationStateAsync();
}