using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Common.Optionals;

namespace ShopApplication.Features.Identity;

public interface IAuthenticationManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<Opt<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken );
    public Task<Opt<bool>> RefreshAuthenticationStateAsync( string? accessToken );
    public Task<Opt<bool>> ClearAuthenticationStateAsync();
}