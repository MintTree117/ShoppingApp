namespace Shop.Types.Orders.Dtos;

public readonly record struct OrderSummaryDto(
    Guid OrderId,
    DateTime DatePlaced,
    DateTime LastUpdate,
    int TotalItems,
    int TotalPrice,
    int OrderStatus );