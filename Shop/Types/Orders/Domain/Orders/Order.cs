using Shop.Types.Common.ValueTypes;
using Shop.Types.Users;

namespace Shop.Types.Orders.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.Empty;
    public string? UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public OrderAddress OrderAddress { get; set; } = null!; // ef core navigation property
    public DateTime DatePlaced { get; set; }
    public DateTime LastUpdate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
    public bool Delayed { get; set; }
    public bool Problem { get; set; }
    public OrderState State { get; set; } = OrderState.Placed;
    public List<OrderGroup> OrderGroups { get; set; } = [];

    public static Order New(
        string? userId,
        Contact contact,
        Address billingAddress,
        Address shippingAddress )
    {
        Guid id = Guid.NewGuid();
        Order o = new() {
            Id = id,
            UserId = userId,
            CustomerName = contact.Name,
            CustomerEmail = contact.Email,
            CustomerPhone = contact.Phone,
            OrderAddress = OrderAddress.From( id, billingAddress, shippingAddress ),
            DatePlaced = DateTime.Now,
            LastUpdate = DateTime.Now
        };

        return o;
    }

    public void CalculatePricing()
    {
        decimal subtotal = decimal.Zero;
        decimal shipping = decimal.Zero;
        decimal discount = decimal.Zero;
        
        foreach ( OrderGroup g in OrderGroups )
        {
            subtotal += g.GetSubtotal();
            shipping += g.GetShippingCost();
            discount += 0;
        }
        
        SubTotal = subtotal;
        ShippingCost = shipping;
        TotalDiscount = discount;
        // TODO: Handle tax percentages more gracefully
        TaxAmount = (subtotal + shipping) * (decimal) 0.13;
        TotalPrice = (SubTotal - TotalDiscount) + ShippingCost + TaxAmount;
    }
    public void CalculateQuantity()
    {
        int total = OrderGroups.Sum( static group => group.OrderLines.Sum( static l => l.Quantity ) );
        TotalQuantity = total;
    }
}