using Shop.Types.Products;
using Shop.Types.Products.Dtos;

namespace Shop.Types.Cart;

public sealed class CartProduct
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal SalePrice { get; set; }
    public bool IsInStock { get; set; }
    public int Quantity { get; set; }
    public int ShippingEstimate { get; set; }
    
    public static CartProduct FromItem( CartItem dto ) =>
        new() {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
    public static CartProduct FromSummary( ProductSummaryDto p, int quantity ) =>
        new() {
            ProductId = p.Id,
            ProductName = p.Name,
            ProductImage = p.Image,
            Price = p.Price,
            SalePrice = p.SalePrice,
            Quantity = quantity
        };
}