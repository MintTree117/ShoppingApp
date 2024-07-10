using Shop.Types.Cart;
using Shop.Types.Common.ValueTypes;
using Shop.Types.Users;

namespace Shop.Types.Orders;

public sealed class OrderPlacementRequest
{
    public Contact Contact { get; set; } = new();
    public Address BillingAddress { get; set; } = new();
    public Address ShippingAddress { get; set; } = new();
    public List<CartItemDto> Items { get; set; } = [];
}