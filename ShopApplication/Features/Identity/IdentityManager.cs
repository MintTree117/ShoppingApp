using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Common;
using ShopApplication.Common.Optionals;
using ShopApplication.Features.Identity.Types;
using ShopApplication.Infrastructure.Storage;

namespace ShopApplication.Features.Identity;

internal sealed class IdentityManager( StorageService storage ) : AuthenticationStateProvider, IIdentityManager
{
    const string Key = "token";
    readonly StorageService _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Val<Authentication> tokenResult = await _storage.Get<Authentication>( Key );

        if (tokenResult.Fails( out tokenResult ))
            return new AuthenticationState( new ClaimsPrincipal() );

        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( tokenResult.Value.JwtToken );
        return new AuthenticationState( claims );
    }
    public async Task<Val<bool>> UpdateAuthenticationState( string token )
    {
        if (string.IsNullOrWhiteSpace( token ))
            return IOptional.Failure( Problem.BadRequest, "Empty token." );

        NotifyChange( token );
        return await _storage.Set( Key, Authentication.With( token ) );
    }
    public async Task<Val<bool>> ClearAuthenticationState()
    {
        NotifyAuthenticationStateChanged( GetNotifyParams( null ) );
        return await _storage.Remove( Key );
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