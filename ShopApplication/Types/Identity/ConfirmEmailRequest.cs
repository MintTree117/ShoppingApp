namespace ShopApplication.Types.Identity;

public readonly record struct ConfirmEmailRequest(
    string UserId,
    string Token )
{
    public static ConfirmEmailRequest With( string userId, string token ) =>
        new( userId, token );
}