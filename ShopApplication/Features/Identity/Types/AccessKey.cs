namespace ShopApplication.Features.Identity.Types;

internal readonly record struct AccessKey(
    string JwtToken )
{
    internal static AccessKey With( string jwtToken ) =>
        new( jwtToken );
}