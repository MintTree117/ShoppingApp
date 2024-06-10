namespace Shop.Infrastructure.Catalog.Search.Dtos;

public record SearchResultsDto(
    int TotalMatches,
    List<SearchItemDto> Results,
    List<int> ShippingEstimates );