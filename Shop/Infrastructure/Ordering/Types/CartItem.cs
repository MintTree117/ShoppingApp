namespace Shop.Infrastructure.Ordering.Types;

public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal SalePrice { get; set; }
    public bool IsInStock { get; set; }
    public int Quantity { get; set; }

    public CartItemDto ToDto() =>
        new ( ProductId, Quantity );
    public static List<CartItemDto> ToDtos( List<CartItem> items )
    {
        List<CartItemDto> dtos = [];
        foreach ( var i in items )
            dtos.Add( new CartItemDto( i.ProductId, i.Quantity ) );
        return dtos;
    }
    public CartItem FromDto( CartItemDto dto ) =>
        new() {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
    decimal DtoPrice() =>
        SalePrice > 0 ? SalePrice : Price;
}