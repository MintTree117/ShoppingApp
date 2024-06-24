namespace Shop.Infrastructure.Catalog.Search.Dtos;

public readonly record struct SearchItemDto(
    Guid Id,
    Guid BrandId,
    string Name,
    string Image,
    bool IsInStock,
    bool IsFeatured,
    decimal Price,
    decimal SalePrice,
    int NumberRatings,
    float Rating );