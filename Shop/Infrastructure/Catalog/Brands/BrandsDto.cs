namespace Shop.Infrastructure.Catalog.Brands;

public readonly record struct BrandsDto(
    List<Brand> Brands,
    Dictionary<Guid, List<Guid>> BrandCategories );