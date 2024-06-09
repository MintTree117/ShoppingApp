using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog.Categories;

public sealed class CategoriesService( HttpService httpService, CategoriesCache categoriesCache )
{
    readonly HttpService http = httpService;
    readonly CategoriesCache cache = categoriesCache;
     Opt<CategoryData> data = categoriesCache.categories;

    public async Task<Opt<CategoryData>> GetCategories()
    {
        if (data.IsOkay)
            return data;

        Opt<List<Category>> fetchResult = await http.TryGetRequest<List<Category>>( "Get Categories" );

        if (!fetchResult.IsOkay)
            return Opt<CategoryData>.None( fetchResult );

        cache.categories = Opt<CategoryData>.With( CategoryData.Create( fetchResult.Data ) );
        return cache.categories;
    }
}