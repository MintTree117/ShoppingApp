using Shop.Types.Cart;
using Shop.Types.Users;

namespace Shop.Types.Orders;

public sealed class OrderPlacementRequest
{
    string CustomerName { get; set; } = string.Empty;
    string CustomerEmail { get; set; } = string.Empty;
    string? CustomerPhone { get; set; } = string.Empty;
    public Address BillingAddress { get; set; } = new();
    public Address ShippingAddress { get; set; } = new();
    public List<CartItemDto> Items { get; set; } = [];
}
