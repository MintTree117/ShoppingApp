namespace Shop.Types.Products.Dtos;

public readonly record struct SearchResultsDto(
    int TotalMatches,
    List<ProductSummaryDto> Results )
{
    public static SearchResultsDto Empty() =>
        new( 0, [] );
}