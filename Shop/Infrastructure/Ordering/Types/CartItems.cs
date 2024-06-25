namespace Shop.Infrastructure.Ordering.Types;

public readonly record struct CartItems(
    List<CartItem> Items )
{
    public int Count() =>
        Items.Count;
    public bool IsEmpty() =>
        Count() <= 0;
    public decimal Price() =>
        Items.Sum( static item => item.SalePrice > 0 
            ? item.SalePrice : item.Price );

    public static CartItems With( List<CartItem> items ) =>
        new( items );
    public static CartItems With( List<CartItemDto> items ) =>
        new(  );
    public static CartItems Empty() =>
        new( [] );
}