using Shop.Infrastructure.Catalog.Products.Models;

namespace Shop.Infrastructure.Ordering.Types;

public class CartItemDto(
    Guid productId,
    int quantity )
{
    public Guid ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
    public static CartItemDto FromProduct( Product p ) =>
        new( p.Id, 1 );
    static decimal GetDtoPrice( Product p ) =>
        p.SalePrice > 0 ? p.SalePrice : p.Price;
}