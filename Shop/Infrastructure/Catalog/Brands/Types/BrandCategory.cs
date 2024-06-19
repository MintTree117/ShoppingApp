namespace Shop.Infrastructure.Catalog.Brands.Types;

public readonly record struct BrandCategory(
    Guid BrandId,
    Guid CategoryId );