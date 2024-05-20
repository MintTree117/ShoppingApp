using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Identity.Types;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure.Identity;

internal sealed class AuthenticationManager( IStorageService storageService ) : AuthenticationStateProvider, IAuthenticationManager
{
    const string AccessKey = "accessToken";
    const string RefreshKey = "refreshToken";
    readonly IStorageService storage = storageService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Opt<AccessKey> tokenResult = await storage.Get<AccessKey>( AccessKey );

        if (tokenResult.Fail( out tokenResult ))
            return new AuthenticationState( new ClaimsPrincipal() );

        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( tokenResult.Data.JwtToken );
        return new AuthenticationState( claims );
    }
    public async Task<Opt<bool>> RefreshAuthenticationStateAsync( string? accessToken )
    {
        if (string.IsNullOrWhiteSpace( accessToken ))
            return IOpt.None( "Empty access token." );
        
        NotifyChange( accessToken );
        
        return await storage.Set( AccessKey, accessToken );
    }
    public async Task<Opt<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken )
    {
        if (string.IsNullOrWhiteSpace( accessToken ))
            return IOpt.None( "Empty access token." );

        if (string.IsNullOrWhiteSpace( refreshToken ))
            return IOpt.None( "Empty refresh token." );

        NotifyChange( accessToken );

        Opt<bool> access = await storage.Set( AccessKey, accessToken );
        Opt<bool> refresh = await storage.Set( RefreshKey, refreshToken );

        return access.IsOkay() && refresh.IsOkay()
            ? IOpt.Okay()
            : IOpt.None( $"{access.Message()} : {refresh.Message()}" );
    }
    public async Task<Opt<bool>> ClearAuthenticationStateAsync()
    {
        NotifyAuthenticationStateChanged( GetNotifyParams( null ) );
        return await storage.Remove( AccessKey );
    }

    void NotifyChange( string token )
    {
        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( token );
        NotifyAuthenticationStateChanged( GetNotifyParams( claims ) );
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
    static ClaimsPrincipal GetIdentityClaimsPrincipal( string jwtToken )
    {
        ClaimsIdentity identity = GetIdentityClaims( jwtToken );
        return new ClaimsPrincipal( identity );
    }
    static Task<AuthenticationState> GetNotifyParams( ClaimsPrincipal? claims ) =>
        Task.FromResult( new AuthenticationState( claims ?? new ClaimsPrincipal() ) );
}