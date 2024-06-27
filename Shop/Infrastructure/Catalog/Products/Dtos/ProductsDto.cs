namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductsDto(
    List<ProductSummaryDto> Products,
    List<int> ShippingDays );