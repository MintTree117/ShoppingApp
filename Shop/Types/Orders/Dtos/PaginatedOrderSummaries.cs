namespace Shop.Types.Orders.Dtos;

public readonly record struct PaginatedOrderSummaries(
    List<OrderSummaryDto> Summaries,
    int TotalCount )
{
    public static PaginatedOrderSummaries Empty() =>
        new( [], 0 );
}