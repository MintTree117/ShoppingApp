using Shop.Types.Products;

namespace Shop.Types.Cart;

public sealed class CartItemDto
{
    // ReSharper disable once UnusedMember.Global
    public CartItemDto() { }

    public CartItemDto( Guid productId, int quantity )
    {
        ProductId = productId;
        Quantity = quantity;
    }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    
    public static CartItemDto FromProduct( ProductModel p ) =>
        new( p.Id, 1 );
}