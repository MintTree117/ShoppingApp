using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure.Authentication;

public sealed class CustomAuthenticationProvider : AuthenticationStateProvider
{
    const string AccessKey = "AccessToken";
    const string RefreshKey = "RefreshToken";
    readonly StorageService _storage;
    
    public CustomAuthenticationProvider( StorageService storageService )
    {
        _storage = storageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Opt<string> tokenResult = await _storage.Get<string>( AccessKey );

        if (tokenResult.Fail( out tokenResult ))
            return new AuthenticationState( new ClaimsPrincipal() );

        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( tokenResult.Data );
        return new AuthenticationState( claims );
    }
    public async Task<Opt<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken )
    {
        if (string.IsNullOrWhiteSpace( accessToken ))
            return IOpt.None( "Invalid access token" );

        if (string.IsNullOrWhiteSpace( refreshToken ))
            return IOpt.None( "Invalid refresh token" );
        
        Opt<bool> accessResult = await _storage.Set( AccessKey, accessToken );
        Opt<bool> refreshResult = await _storage.Set( RefreshKey, refreshToken );

        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( accessToken );
        NotifyAuthenticationStateChanged( GetNotifyParams( claims ) );
        
        return accessResult.IsOkay && refreshResult.IsOkay
            ? IOpt.Okay()
            : IOpt.None( accessResult.Message() + refreshResult.Message() );
    }
    public async Task<Opt<bool>> ClearAuthenticationStateAsync()
    {
        Opt<bool> result1 = await _storage.Remove( AccessKey );
        Opt<bool> result2 = await _storage.Remove( RefreshKey );
        NotifyAuthenticationStateChanged( GetNotifyParams( null ) );
        return result1.IsOkay && result2.IsOkay
            ? IOpt.Okay()
            : IOpt.None( result1.Message() + result2.Message() );
    }
    public async Task<Opt<string>> GetAccessToken() => 
        await _storage.Get<string>( AccessKey );
    public async Task<Opt<string>> GetRefreshToken() =>
        await _storage.Get<string>( RefreshKey );
    
    static ClaimsPrincipal GetIdentityClaimsPrincipal( string jwtToken )
    {
        ClaimsIdentity identity = GetIdentityClaims( jwtToken );
        return new ClaimsPrincipal( identity );
    }
    static ClaimsIdentity GetIdentityClaims( string jwtToken )
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken( jwtToken );

        Claim? nameIdentifierClaim = token.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier );
        Claim? emailClaim = token.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Email );
        Claim? nameClaim = token.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Name );

        return new ClaimsIdentity( new[] {
            nameIdentifierClaim ?? new Claim( ClaimTypes.NameIdentifier, "" ),
            emailClaim ?? new Claim( ClaimTypes.Email, "" ),
            nameClaim ?? new Claim( ClaimTypes.Name, "" )
        }, "token" );
    }
    static async Task<AuthenticationState> GetNotifyParams( ClaimsPrincipal? claims ) =>
        await Task.FromResult( new AuthenticationState( claims ?? new ClaimsPrincipal() ) );
}