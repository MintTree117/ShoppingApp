namespace Shop.Types.Orders.Dtos;

public readonly record struct OrderFullDto(
    OrderSummaryDto Summary,
    List<OrderLineDto> OrderLines );