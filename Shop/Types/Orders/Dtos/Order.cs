using Shop.Types.Common.ValueTypes;
using Shop.Types.Users;

namespace Shop.Types.Orders.Dtos;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.Empty;
    public string? UserId { get; set; } = string.Empty;
    public Contact Contact { get; set; }
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
    public DateTime DatePlaced { get; set; }
    public DateTime LastUpdate { get; set; }
    //public Pricing Pricing { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
    public bool Delayed { get; set; }
    public bool Problem { get; set; }
    public OrderState State { get; set; } = OrderState.Placed;
    public List<OrderGroup> OrderGroups { get; set; } = [];

    public static Order New(
        string? userId,
        Contact contact,
        Address shippingAddress,
        Address billingAddress ) =>
        new() {
            Id = Guid.NewGuid(),
            UserId = userId,
            Contact = contact,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            DatePlaced = DateTime.Now,
            LastUpdate = DateTime.Now
        };
}