namespace Shop.Infrastructure.Catalog.Brands;

public readonly record struct BrandData(
    IReadOnlyDictionary<Guid, Brand> BrandsById,
    IReadOnlyDictionary<Guid, IReadOnlyList<Brand>> BrandsByCategoryId );