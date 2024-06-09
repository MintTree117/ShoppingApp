using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Catalog.Categories;

public sealed class CategoriesCache( HttpService http, StorageService storage ) :
    MemoryCache<CategoriesCollection>( "Categories", storage, TimeSpan.FromHours( 24 ) ) // Singleton
{
    readonly HttpService _http = http;
    bool _isFetching = false;
    
    public async Task<Reply<CategoriesCollection>> GetCategories()
    {
        int count = 0;
        while ( _isFetching && count < 3 ) {
            await Task.Delay( 1000 );
            count++;
        }
        
        _isFetching = true;
        Reply<CategoriesCollection> cacheReply = await GetCache();
        if (cacheReply.IsOkay)
            return cacheReply;

        Reply<List<CategoryDto>> fetchReply = await _http.TryGetRequest<List<CategoryDto>>( Consts.ApiGetCategories );

        if (!fetchReply.IsOkay)
            return Reply<CategoriesCollection>.None( fetchReply );

        CategoriesCollection data = CategoriesCollection.From( fetchReply.Data );

        Reply<bool> setReply = await SetCache( data );
        _isFetching = false;
        return Reply<CategoriesCollection>.With( data );
    }
}