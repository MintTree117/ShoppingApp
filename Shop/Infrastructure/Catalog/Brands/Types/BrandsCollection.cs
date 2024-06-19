namespace Shop.Infrastructure.Catalog.Brands.Types;

public sealed record BrandsCollection
{
    public BrandsCollection( Dictionary<Guid, Brand> brandsById, Dictionary<Guid, HashSet<Brand>> brandsByCategory )
    {
        BrandsById = brandsById;

        Dictionary<Guid, HashSet<Brand>> dictionary = [];
        foreach ( var kvp in brandsByCategory )
            dictionary.Add( kvp.Key, kvp.Value );
        BrandsByCategory = dictionary;
    }

    public Dictionary<Guid, Brand> BrandsById { get; init; }
    public Dictionary<Guid, HashSet<Brand>> BrandsByCategory { get; init; }

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