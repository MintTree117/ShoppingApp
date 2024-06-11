using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsCache( HttpService http, StorageService storage ) 
    : MemoryCache<BrandsCollection>( "Brands", storage, TimeSpan.FromHours( 24 ) ) // Singleton
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
        Reply<BrandsCollection> cacheReply = await GetCache();
        if (cacheReply.IsOkay)
        {
            _isFetching = false;
            return cacheReply;
        }

        Reply<BrandsDto> fetchReply = await _http.GetAsync<BrandsDto>( Consts.ApiGetBrands );

        if (!fetchReply.IsOkay)
        {
            _isFetching = false;
            Logger.LogError( fetchReply.Message() );
            return Reply<BrandsCollection>.False( "Failed to fetch brands from server" );
        }

        BrandsCollection data = BrandsCollection.From( fetchReply.Data );

        Reply<bool> setReply = await SetCache( data );
        _isFetching = false;
        return Reply<BrandsCollection>.True( data );
    }
}