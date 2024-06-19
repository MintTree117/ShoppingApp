namespace Shop.Infrastructure.Catalog.Search.Dtos;

public readonly record struct SearchResultsDto(
    int TotalMatches,
    List<SearchItemDto> Results,
    List<int> ShippingDays );