namespace Shop.Infrastructure.Catalog;

public sealed class CategoryData( Dictionary<Guid, Category> dict, List<Category> primes )
{
    public readonly IReadOnlyDictionary<Guid, Category> Categories = dict;
    public readonly IReadOnlyList<Category> PrimaryCategories = primes;

    public static CategoryData Create( List<Category> dtos )
    {
        Dictionary<Guid, Category> dictionary = [];
        List<Category> primaries = [];

        foreach ( Category c in dtos ) {
            dictionary.TryAdd( c.Id, c );

            if (c.Tier == 1)
                primaries.Add( c );
        }

        foreach ( Category c in dtos ) {
            if (c.ParentId is null)
                continue;
            Category parent = dictionary[c.ParentId.Value];
            parent.Children.Add( c );
        }

        return new CategoryData( dictionary, primaries );
    }
    public static CategoryData None() => 
        new( new Dictionary<Guid, Category>(), [] );
}