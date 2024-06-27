namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductDetailsDto(
    Guid Id,
    Guid BrandId,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    decimal Price,
    decimal SalePrice,
    float Rating,
    int NumberRatings,
    int ShippingDays,
    List<Guid>? CategoryIds,
    string? Description,
    string? Xml );