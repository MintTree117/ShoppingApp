using Shop.Types.Users;

namespace Shop.Types.Orders.Domain.Orders;

public sealed class OrderAddress
{
    public required Guid Id { get; set; }
    public required Guid OrderId { get; set; }
    public required string BillingAddressName { get; set; } = string.Empty;
    public required int BillingPosX { get; set; }
    public required int BillingPosY { get; set; }
    public required string ShippingAddressName { get; set; } = string.Empty;
    public required int ShippingPosX { get; set; }
    public required int ShippingPosY { get; set; }

    public static OrderAddress From( Guid orderId, Address billing, Address shipping ) =>
        new() {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            BillingAddressName = billing.Name,
            BillingPosX = billing.PosX,
            BillingPosY = billing.PosY,
            ShippingAddressName = shipping.Name,
            ShippingPosX = shipping.PosX,
            ShippingPosY = shipping.PosY
        };
}