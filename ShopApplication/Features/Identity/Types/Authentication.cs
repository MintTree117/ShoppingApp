namespace ShopApplication.Features.Identity.Types;

internal readonly record struct Authentication(
    string JwtToken )
{
    internal static Authentication With( string jwtToken ) =>
        new( jwtToken );
}