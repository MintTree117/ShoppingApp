using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Shop.Infrastructure.Authentication.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Authentication;

public sealed class AuthProvider : AuthenticationStateProvider
{
    const string AccessKey = "AccessToken";
    const string RefreshKey = "RefreshToken";

    readonly Jwt _rules;
    readonly HttpService _http;
    readonly StorageService _storage;
    readonly ILogger<AuthProvider> _logger;
    
    public AuthProvider( IConfiguration config, HttpService http, StorageService storage, ILogger<AuthProvider> logger )
    {
        _rules = GetJwtRules( config );
        _http = http;
        _storage = storage;
        _logger = logger;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Opt<string> aTokenResult = await _storage.Get<string>( AccessKey );
        if (!aTokenResult.Fail( out aTokenResult ) && !ShouldRefresh( aTokenResult.Data, _rules )) 
            return new AuthenticationState( GetIdentityClaimsPrincipal( aTokenResult.Data ) );
        
        Opt<string> rTokenResult = await _storage.Get<string>( RefreshKey );
        if (!rTokenResult.IsOkay)
            return new AuthenticationState( new ClaimsPrincipal() );

        RefreshRequest request = new( aTokenResult.Data, rTokenResult.Data );
        Opt<RefreshReply> refreshResult = await _http.TryPostRequest<RefreshReply>( Urls.ApiLoginRefresh, request );

        if (!refreshResult.IsOkay)
            return new AuthenticationState( new ClaimsPrincipal() );

        Opt<bool> setResult = await SetAuthenticationStateAsync( AuthRefreshEventArgs.With( refreshResult.Data.NewAccessToken, rTokenResult.Data ) );
        return !setResult.IsOkay 
            ? new AuthenticationState( new ClaimsPrincipal() ) 
            : new AuthenticationState( GetIdentityClaimsPrincipal( aTokenResult.Data ) );

    }
    internal async Task<Opt<bool>> SetAuthenticationStateAsync( AuthRefreshEventArgs args )
    {
        if (string.IsNullOrWhiteSpace( args.accessToken ))
            return IOpt.None( "Invalid access token" );

        if (string.IsNullOrWhiteSpace( args.refreshToken ))
            return IOpt.None( "Invalid refresh token" );
        
        ClaimsPrincipal claims = GetIdentityClaimsPrincipal( args.accessToken );
        NotifyAuthenticationStateChanged( GetNotifyParams( claims ) );
        return await SetAuthentication( args );
    }
    internal async Task<Opt<bool>> ClearAuthenticationStateAsync()
    {
        NotifyAuthenticationStateChanged( GetNotifyParams( null ) );
        return await ClearAuthentication();
    }
    internal async Task<Opt<string>> GetAccessToken() =>
        await _storage.Get<string>( AccessKey );

    async Task<Opt<bool>> SetAuthentication( AuthRefreshEventArgs args )
    {
        Opt<bool> accessResult = await _storage.Set( AccessKey, args.accessToken );
        Opt<bool> refreshResult = await _storage.Set( RefreshKey, args.refreshToken );

        return accessResult.IsOkay && refreshResult.IsOkay
            ? IOpt.Okay()
            : IOpt.None( accessResult.Message() + refreshResult.Message() );
    }
    async Task<Opt<bool>> ClearAuthentication()
    {
        Opt<bool> result1 = await _storage.Remove( AccessKey );
        Opt<bool> result2 = await _storage.Remove( RefreshKey );

        return result1.IsOkay && result2.IsOkay
            ? IOpt.Okay()
            : IOpt.None( result1.Message() + result2.Message() );
    }
    
    static async Task<AuthenticationState> GetNotifyParams( ClaimsPrincipal? claims ) =>
        await Task.FromResult( new AuthenticationState( claims ?? new ClaimsPrincipal() ) );
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
        Claim? nameClaim = token.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Name );

        return new ClaimsIdentity( new[] {
            nameIdentifierClaim ?? new Claim( ClaimTypes.NameIdentifier, "None" ),
            nameClaim ?? new Claim( ClaimTypes.Name, "None" )
        }, "token" );
    }

    static bool ShouldRefresh( string token, Jwt rules )
    {
        try {
            Opt<JwtSecurityToken> jwt = ParseJwtFromString( token, rules );
            if (!jwt.IsOkay)
                return true;

            long expiryDateUnix = long.Parse( jwt.Data.Claims.First( claim => claim.Type == "exp" ).Value );
            DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds( expiryDateUnix ).UtcDateTime;

            DateTime currentTime = DateTime.UtcNow;
            TimeSpan timeDifference = expiryDateTime - currentTime;

            return timeDifference.TotalMinutes <= 5;
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return true;
        }
    }
    static Jwt GetJwtRules( IConfiguration config ) => new() {
        Key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( config.GetOrThrow( "Identity:Jwt:Key" ) ) ),
        Audience = config.GetOrThrow( "Identity:Jwt:Audience" ),
        Issuer = config.GetOrThrow( "Identity:Jwt:Issuer" ),
        ValidateAudience = config.GetSection( "Identity:Jwt:ValidateAudience" ).Get<bool>(),
        ValidateIssuer = config.GetSection( "Identity:Jwt:ValidateIssuer" ).Get<bool>(),
        ValidateIssuerSigningKey = config.GetSection( "Identity:Jwt:ValidateIssuerSigningKey" ).Get<bool>(),
        AccessLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:AccessLifetime" ) ),
        RefreshLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:RefreshLifetime" ) )
    };
    static Opt<JwtSecurityToken> ParseJwtFromString( string token, Jwt rules )
    {
        try {
            JwtSecurityTokenHandler tokenHandler = new();
            TokenValidationParameters validationParameters = new() {
                ValidateIssuerSigningKey = rules.ValidateIssuerSigningKey,
                IssuerSigningKey = rules.Key,
                ValidateIssuer = rules.ValidateIssuer,
                ValidIssuer = rules.Issuer,
                ValidateAudience = rules.ValidateAudience,
                ValidAudience = rules.Audience,
                ClockSkew = TimeSpan.Zero // Remove delay of token expiration
            };

            tokenHandler.ValidateToken( token, validationParameters, out SecurityToken securityToken );

            return securityToken is JwtSecurityToken jwtSecurityToken
                ? Opt<JwtSecurityToken>.With( jwtSecurityToken )
                : Opt<JwtSecurityToken>.None( "Jwt string is invalid." );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<JwtSecurityToken>.None( "An exception was thrown while parsing a jwt." );
        }
    }
    
    readonly record struct RefreshRequest(
        string? OldAccessToken,
        string? RefreshToken );
    readonly record struct RefreshReply(
        string? NewAccessToken );
}