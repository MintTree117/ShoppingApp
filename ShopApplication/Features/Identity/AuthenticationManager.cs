using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ShopApplication.Common;
using ShopApplication.Common.Optionals;
using ShopApplication.Features.Identity.Types;
using ShopApplication.Infrastructure.Storage;

namespace ShopApplication.Features.Identity;

internal sealed class AuthenticationManager( StorageService storage ) : AuthenticationStateProvider, IAuthenticationManager
{
    const string AccessKey = "accessToken";
    const string RefreshKey = "refreshToken";
    readonly StorageService _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Val<AccessKey> tokenResult = await _storage.GetVal<AccessKey>( AccessKey );

        if (tokenResult.Fails( out tokenResult ))
            return new AuthenticationState( new ClaimsPrincipal() );

        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( tokenResult.Value.JwtToken );
        return new AuthenticationState( claims );
    }
    public async Task<Val<bool>> RefreshAuthenticationStateAsync( string? accessToken )
    {
        if (string.IsNullOrWhiteSpace( accessToken ))
            return IOptional.Failure( Problem.BadRequest, "Empty access token." );
        
        NotifyChange( accessToken );
        
        return await _storage.Set( AccessKey, accessToken );
    }
    public async Task<Val<bool>> SetAuthenticationStateAsync( string? accessToken, string? refreshToken )
    {
        if (string.IsNullOrWhiteSpace( accessToken ))
            return IOptional.Failure( Problem.BadRequest, "Empty access token." );

        if (string.IsNullOrWhiteSpace( refreshToken ))
            return IOptional.Failure( Problem.BadRequest, "Empty refresh token." );

        NotifyChange( accessToken );
        
        Val<bool> access = await _storage.Set( AccessKey, accessToken );
        Val<bool> refresh = await _storage.Set( RefreshKey, refreshToken );

        return access.IsSuccess() && refresh.IsSuccess()
            ? IOptional.Success()
            : IOptional.Failure( Problem.IO, $"{access.PrintDetails()} : {refresh.PrintDetails()}" );
    }
    public async Task<Val<bool>> ClearAuthenticationStateAsync()
    {
        NotifyAuthenticationStateChanged( GetNotifyParams( null ) );
        return await _storage.Remove( AccessKey );
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