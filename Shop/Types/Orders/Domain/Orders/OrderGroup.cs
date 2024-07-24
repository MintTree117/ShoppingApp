using System.Text.Json.Serialization;

namespace Shop.Types.Orders.Domain.Orders;

public sealed class OrderGroup
{
    public Guid Id { get; set; }
    [JsonIgnore] public Order Order { get; set; } = null!;
    public Guid OrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime LastUpdated { get; set; }
    public OrderState State { get; set; }
    public List<OrderLine> OrderLines { get; set; } = [];

    public void Update( OrderState state )
    {
        State = state;
        LastUpdated = DateTime.Now;
    }

    public decimal GetSubtotal() =>
        OrderLines.Sum( static o => o.UnitPrice );
    public decimal GetShippingCost() =>
        OrderLines.Sum( static o => o.ShippingCost );
}