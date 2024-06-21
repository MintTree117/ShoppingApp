namespace Shop.Infrastructure.Catalog.Search.Dtos;

public readonly record struct SearchItemDto(
    Guid ProductId,
    Guid BrandId,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    decimal Price,
    decimal SalePrice,
    int NumberSold,
    int NumberRatings,
    float Rating );