using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog.Categories;

public sealed class CategoriesService( HttpService http, CategoriesCache cache )
{
    readonly HttpService _http = http;
    readonly CategoriesCache _cache = cache;

    public async Task<Reply<CategoriesCollection>> GetCategories()
    {
        Reply<CategoriesCollection> cacheReply = await _cache.Get();
        if (cacheReply.IsOkay)
            return cacheReply;

        Reply<List<CategoryDto>> fetchReply = await _http.TryGetRequest<List<CategoryDto>>( "Get Categories" );

        if (!fetchReply.IsOkay)
            return Reply<CategoriesCollection>.None( fetchReply );

        CategoriesCollection data = CategoriesCollection.From( fetchReply.Data );

        Reply<bool> setReply = await _cache.Set( data );
        return Reply<CategoriesCollection>.With( data );
    }
}