namespace Shop.Types.Orders.Domain.Orders;

public sealed class WarehouseOrderGroup
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid OrderId { get; set; }
    public Guid OrderGroupId { get; set; }

    public WarehouseOrderGroup() { }

    public WarehouseOrderGroup( Guid id, Guid warehouseId, Guid orderId, Guid orderGroupId )
    {
        Id = id;
        WarehouseId = warehouseId;
        OrderId = orderId;
        OrderGroupId = orderGroupId;
    }

    public static List<WarehouseOrderGroup> FromOrder( Order order )
    {
        List<WarehouseOrderGroup> groups = [];
        foreach ( var g in order.OrderGroups )
            groups.Add( new WarehouseOrderGroup( Guid.NewGuid(), g.WarehouseId, g.OrderId, g.Id ) );
        return groups;
    }
}