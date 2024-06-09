namespace Shop.Infrastructure.Catalog.Brands.Types;

public readonly record struct BrandsDto(
    List<Brand> Brands,
    Dictionary<Guid, List<Guid>> BrandCategories );