namespace ShopApplication.Features.Identity.Types;

public readonly record struct ConfirmEmailReply(
    string ConfirmedEmail,
    string ConfirmedUsername )
{
    public static ConfirmEmailReply Empty() =>
        new( string.Empty, string.Empty );
}