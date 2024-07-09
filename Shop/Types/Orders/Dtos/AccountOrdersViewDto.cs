namespace Shop.Types.Orders.Dtos;

public readonly record struct AccountOrdersViewDto(
    int TotalCount,
    List<AccountOrderViewDto> Orders );