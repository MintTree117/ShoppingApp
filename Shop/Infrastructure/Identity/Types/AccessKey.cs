namespace Shop.Infrastructure.Identity.Types;

internal record AccessKey(
    string JwtToken )
{
    internal static AccessKey With( string jwtToken ) =>
        new( jwtToken );
}