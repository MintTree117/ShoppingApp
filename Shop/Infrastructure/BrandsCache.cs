using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Types.Brands;
using Shop.Types.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure;

public sealed class BrandsCache( HttpService http, StorageService storage ) 
    : MemoryCache<BrandsCollection>( "Brands", storage, TimeSpan.FromHours( 24 ) ) // SINGLETON
{
    readonly HttpService _http = http;
    bool _isFetching = false;
    
    public async Task<Reply<BrandsCollection>> GetBrands()
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

        var fetchReply = await _http.GetAsync<BrandsDto>( _http.CatalogApi( Consts.ApiGetBrands ) );
        if (!fetchReply)
        {
            _isFetching = false;
            Logger.LogError( fetchReply.GetMessage() );
            return Reply<BrandsCollection>.ServerError( "Failed to fetch brands from server." );
        }

        BrandsCollection data = BrandsCollection.From( fetchReply.Data );
        var setReply = await SetCache( data );
        
        if (!setReply)
            Logger.LogError( $"Failed to store brands in browser cache. {setReply}" );
        else
            Logger.Log( $"Successfully fetched brands from server and stored in browser cache. {setReply}" );
        
        _isFetching = false;
        return Reply<BrandsCollection>.Success( data );
    }
}