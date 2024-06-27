namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductSummaryDto(
    Guid Id,
    Guid BrandId,
    bool IsFeatured,
    bool IsInStock,
    string Name,
    string Image,
    decimal Price,
    decimal SalePrice,
    float Rating,
    int NumberRatings
);