using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.Optionals;

namespace Shop.Features.Identity;

public interface IAuthenticationManager
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();
    public Task<Opt<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken );
    public Task<Opt<bool>> RefreshAuthenticationStateAsync( string? accessToken );
    public Task<Opt<bool>> ClearAuthenticationStateAsync();
}