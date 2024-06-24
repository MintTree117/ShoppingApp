namespace Shop.Infrastructure.Catalog.Search.Dtos;

public readonly record struct SearchItemDto(
    Guid Id,
    Guid BrandId,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    decimal Price,
    decimal SalePrice,
    int NumberRatings,
    float Rating );