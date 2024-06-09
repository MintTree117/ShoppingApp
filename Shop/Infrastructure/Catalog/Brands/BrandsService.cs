using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsService( HttpService http, BrandsCache cache )
{
    readonly HttpService _http = http;
    readonly BrandsCache _cache = cache;

    public async Task<Reply<BrandsCollection>> GetBrands()
    {
        Reply<BrandsCollection> cacheReply = await _cache.Get();
        if (cacheReply.IsOkay)
            return cacheReply;

        Reply<BrandsDto> fetchReply = await _http.TryGetRequest<BrandsDto>( "Get Brands" );

        if (!fetchReply.IsOkay)
            return Reply<BrandsCollection>.None( fetchReply );

        BrandsCollection data = BrandsCollection.From( fetchReply.Data );

        Reply<bool> setReply = await _cache.Set( data );
        return Reply<BrandsCollection>.With( data );
    }
}