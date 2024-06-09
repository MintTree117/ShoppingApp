using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog.Categories;

public sealed class CategoriesService( HttpService httpService, CategoriesCache categoriesCache )
{
    readonly HttpService _http = httpService;
    readonly CategoriesCache _cache = categoriesCache;

    public async Task<Opt<CategoryData>> GetCategories()
    {
        if (_cache.Categories.IsOkay)
            return _cache.Categories;

        Opt<List<Category>> fetchResult = await _http.TryGetRequest<List<Category>>( "Get Categories" );

        if (!fetchResult.IsOkay)
            return Opt<CategoryData>.None( fetchResult );

        _cache.Categories = Opt<CategoryData>.With( CategoryData.Create( fetchResult.Data ) );
        return _cache.Categories;
    }
}