namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductDto(
    Guid Id,
    Guid BrandId,
    string Name,
    string Image,
    bool IsInStock,
    bool IsFeatured,
    int ShippingDays,
    decimal Price,
    decimal SalePrice,
    int NumberRatings,
    float Rating,
    List<Guid>? CategoryIds,
    string? Description,
    string? Xml );