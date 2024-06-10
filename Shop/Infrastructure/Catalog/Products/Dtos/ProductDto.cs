namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductDto(
    Guid ProductId,
    Guid BrandId,
    List<Guid> CategoryIds,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    int ShippingEstimate,
    decimal Price,
    decimal SalePrice,
    string Description,
    string Xml );