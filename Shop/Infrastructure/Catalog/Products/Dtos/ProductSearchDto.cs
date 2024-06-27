namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductSearchDto(
    int TotalMatches,
    List<ProductSummaryDto> Results,
    List<int> ShippingDays );