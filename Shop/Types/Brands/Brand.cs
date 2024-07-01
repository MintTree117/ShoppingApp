namespace Shop.Types.Brands;

public sealed record Brand(
    Guid Id,
    string Name )
{
    public static Brand Default() =>
        new( Guid.Empty, string.Empty );
}