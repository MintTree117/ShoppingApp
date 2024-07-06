using Shop.Types.Cart;
using Shop.Types.Users;

namespace Shop.Types.Orders;

public sealed class OrderModel
{
    public AddressModel? BillingAddress { get; set; }
    public AddressModel? ShippingAddress { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}