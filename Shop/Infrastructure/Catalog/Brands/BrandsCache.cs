using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsCache( HttpService http, StorageService storage ) :
    MemoryCache<BrandsCollection>( "Brands", storage, TimeSpan.FromHours( 24 ) )
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
            return cacheReply;

        Reply<BrandsDto> fetchReply = await _http.TryGetRequest<BrandsDto>( Consts.ApiGetBrands );

        if (!fetchReply.IsOkay)
            return Reply<BrandsCollection>.None( fetchReply );

        BrandsCollection data = BrandsCollection.From( fetchReply.Data );

        Reply<bool> setReply = await SetCache( data );
        _isFetching = false;
        return Reply<BrandsCollection>.With( data );
    }
}