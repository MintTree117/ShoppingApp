namespace Shop.Infrastructure.Catalog.Brands;

public record BrandData(
    IReadOnlyDictionary<Guid, Brand> BrandsById,
    IReadOnlyDictionary<Guid, IReadOnlyList<Brand>> BrandsByCategoryId );