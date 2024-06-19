namespace Shop.Infrastructure.Catalog.Brands.Types;

public sealed record BrandsCollection
{
    public BrandsCollection( Dictionary<Guid, Brand> brands, Dictionary<Guid, HashSet<Brand>> brandCategories )
    {
        BrandsById = brands;

        Dictionary<Guid, IReadOnlySet<Brand>> dictionary = [];
        foreach ( var kvp in brandCategories )
            dictionary.Add( kvp.Key, kvp.Value );
        BrandsByCategory = dictionary;
    }

    public readonly IReadOnlyDictionary<Guid, Brand> BrandsById = new Dictionary<Guid, Brand>();
    public readonly IReadOnlyDictionary<Guid, IReadOnlySet<Brand>> BrandsByCategory = new Dictionary<Guid, IReadOnlySet<Brand>>();

    public static BrandsCollection From( BrandsDto dto )
    {
        Dictionary<Guid, Brand> brandsById = [];
        foreach ( Brand b in dto.Brands )
            brandsById.TryAdd( b.Id, b );

        Dictionary<Guid, HashSet<Brand>> brandsByCategory = [];
        foreach ( BrandCategory bc in dto.BrandCategories ) {
            if (!brandsById.TryGetValue( bc.BrandId, out Brand? b ))
                continue;
            if (!brandsByCategory.TryGetValue( bc.CategoryId, out HashSet<Brand>? brands ))
            {
                brands = [];
                brandsByCategory.Add( bc.CategoryId, brands );
            }
            brands.Add( b );
        }

        return new BrandsCollection( brandsById, brandsByCategory );
    }
    public static BrandsCollection None() =>
        new( [], [] );
}