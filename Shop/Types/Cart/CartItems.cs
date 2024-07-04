namespace Shop.Types.Cart;

public sealed class CartItems(
    List<CartItem> items )
{
    public List<CartItem> Items { get; private set; } = items;
    
    public static CartItems With( List<CartItem> dtos ) =>
        new( dtos );
    public static CartItems Empty() =>
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
    public List<Guid> GetIds() =>
        Items.Select( static i => i.ProductId ).ToList();
    public int Count() =>
        Items.Count;
    public int Quantity( Guid productId )
    {
        var item = Items.FirstOrDefault( c => c.ProductId == productId );
        return item?.Quantity ?? 0;
    }
    public void Add( CartItem dto )
    {
        Items.Add( dto );
    }
    public void Set( CartItem dto )
    {
        foreach ( CartItem d in Items.Where( d => d.ProductId == dto.ProductId ) )
            d.Quantity = dto.Quantity;
    }
    public void Delete( Guid productId )
    {
        var item = Items.FirstOrDefault( i => i.ProductId == productId );
        if (item is not null)
            Items.Remove( item );
    }
}