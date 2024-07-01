namespace Shop.Types.Brands;

public readonly record struct BrandsDto(
    List<Brand> Brands,
    Dictionary<Guid, HashSet<Guid>> BrandCategories );