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
    public bool Contains( Guid productId, out int quantity )
    {
        quantity = 0;
        if (Items.All( c => c.ProductId != productId ))
            return false;
        
        var item = Items.FirstOrDefault( i => i.ProductId == productId );
        if (item is null)
            return false;
        quantity = item.Quantity;
        return true;
    }
    public int Quantity( Guid productId )
    {
        var item = Items.FirstOrDefault( c => c.ProductId == productId );
        return item?.Quantity ?? 0;
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