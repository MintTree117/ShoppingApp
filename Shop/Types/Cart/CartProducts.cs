namespace Shop.Types.Cart;

public readonly record struct CartProducts(
    List<CartProduct> Items )
{
    public int Count() =>
        Items.Count;
    public bool IsEmpty() =>
        Count() <= 0;
    public decimal Price() =>
        Items.Sum( static item => item.SalePrice > 0 
            ? item.SalePrice : item.Price );

    public static CartProducts FromItems( CartItems items )
    {
        List<CartProduct> products = [];
        foreach ( var i in items.Items )
            products.Add( CartProduct.FromItem( i ) );
        return With( products );
    }
    public static CartProducts With( List<CartProduct> items ) =>
        new( items );
    public static CartProducts With( List<CartItem> items ) =>
        new(  );
    public static CartProducts Empty() =>
        new( [] );
}