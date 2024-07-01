namespace Shop.Types.Brands;

public sealed record BrandsCollection
{
    public BrandsCollection( Dictionary<Guid, Brand> brandsById, Dictionary<Guid, HashSet<Brand>> brandsByCategoryId )
    {
        BrandsById = brandsById;

        Dictionary<Guid, HashSet<Brand>> dictionary = [];
        foreach ( var kvp in brandsByCategoryId )
            dictionary.Add( kvp.Key, kvp.Value );
        BrandsByCategoryId = dictionary;
    }

    public Dictionary<Guid, Brand> BrandsById { get; init; }
    public Dictionary<Guid, HashSet<Brand>> BrandsByCategoryId { get; init; }

    public static BrandsCollection From( BrandsDto dto )
    {
        Dictionary<Guid, Brand> brandsById = [];
        foreach ( Brand b in dto.Brands )
            brandsById.TryAdd( b.Id, b );

        Dictionary<Guid, HashSet<Brand>> brandsByCategory = [];
        foreach ( var kvp in dto.BrandCategories ) 
        {
            if (brandsByCategory.ContainsKey( kvp.Key ))
                continue;

            HashSet<Brand> brands = [];
            foreach ( Guid id in kvp.Value )
                if (brandsById.TryGetValue( id, out Brand? b ))
                    brands.Add( b );

            brandsByCategory.Add( kvp.Key, brands );
        }

        return new BrandsCollection( brandsById, brandsByCategory );
    }
    public static BrandsCollection None() =>
        new( [], [] );
}