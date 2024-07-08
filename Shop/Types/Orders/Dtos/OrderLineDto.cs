using Shop.Types.Cart;

namespace Shop.Types.Orders.Dtos;

public readonly record struct OrderLineDto(
    Guid Id,
    Guid OrderId,
    DateTime DateCreated,
    DateTime LastUpdate,
    int Status,
    List<CartItemDto> Items );