namespace ShopApplication.Features.Identity.Types;

internal readonly record struct UserSession(
    string Id,
    string Email,
    string Name )
{
    internal static UserSession With( string id, string email, string name ) =>
        new( id, email, name );
}