using Shop.Types.Products;

namespace Shop.Types.Cart;

public sealed class CartItem(
    Guid productId,
    int quantity )
{
    public Guid ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
    
    public static CartItem FromProduct( ProductModel p ) =>
        new( p.Id, 1 );
    public static CartItem FromCartProduct( CartProduct p ) =>
        new( p.ProductId, p.Quantity );
}