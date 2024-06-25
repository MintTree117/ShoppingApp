namespace Shop.Infrastructure.Ordering.Types;

public sealed class CartSummary(
    List<CartItemDto> Items )
{
    public static CartSummary With( List<CartItemDto> dtos ) =>
        new( dtos );
    public static CartSummary Empty() =>
        new( [] );
    public bool Contains( Guid productId )
    {
        return Items.Any( c => c.ProductId == productId );
    }
    public void Add( CartItemDto dto )
    {
        Items.Add( dto );
    }
    public void Set( CartItemDto dto )
    {
        foreach ( CartItemDto d in Items.Where( d => d.ProductId == dto.ProductId ) )
            d.Quantity = dto.Quantity;
    }
    public void Delete( Guid productId )
    {
        var item = Items.FirstOrDefault( i => i.ProductId == productId );
        if (item is not null)
            Items.Remove( item );
    }
}