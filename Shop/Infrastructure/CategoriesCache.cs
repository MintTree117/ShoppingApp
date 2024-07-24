using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Types.Categories;
using Shop.Types.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure;

public sealed class CategoriesCache( HttpService http, StorageService storage ) :
    MemoryCache<CategoriesCollection>( "Categories", storage, TimeSpan.FromHours( 24 ) ) // SINGLETON
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
        var cacheReply = await GetCache();
        if (cacheReply)
        {
            _isFetching = false;
            return cacheReply;
        }

        var fetchReply = await _http.GetAsync<List<CategoryDto>>( _http.Catalog( Consts.ApiGetCategories ) );
        if (!fetchReply)
        {
            _isFetching = false;
            Logger.LogError( "Failed to fetch categories from server." );
            return Reply<CategoriesCollection>.Fail( fetchReply );   
        }

        CategoriesCollection data = CategoriesCollection.From( fetchReply.Data );
        var setReply = await SetCache( data );
        
        if (!setReply) 
            Logger.LogError( $"Failed to store categories in browser cache. {setReply}" );
        else
            Logger.Log( $"Successfully fetched categories from server and stored in browser cache. {setReply}" );
        
        _isFetching = false;
        return Reply<CategoriesCollection>.Success( data );
    }
}