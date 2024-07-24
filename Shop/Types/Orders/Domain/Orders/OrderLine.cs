using System.Text.Json.Serialization;

namespace Shop.Types.Orders.Domain.Orders;

public sealed class OrderLine
{
    public OrderLine() { }
    public OrderLine( Guid orderId, Guid warehouseId )
    {
        OrderId = orderId;
        WarehouseId = warehouseId;
    }

    public Guid Id { get; set; }
    [JsonIgnore] public OrderGroup OrderGroup { get; set; } = new();
    public Guid OrderId { get; set; }
    public Guid OrderGroupId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal UnitDiscount { get; set; }
    public decimal ShippingCost { get; set; }
    public int Quantity { get; set; }
}