namespace Shop.Types.Orders.Domain.Orders;

public sealed class Warehouse
{
    public Guid Id { get; set; } = Guid.Empty;
    public int PosX { get; set; }
    public int PosY { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
}