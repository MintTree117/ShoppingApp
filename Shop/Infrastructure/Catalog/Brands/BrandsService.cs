using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsService( HttpService http, BrandsCache cache )
{
    readonly HttpService _http = http;
    readonly BrandsCache _cache = cache;

    public async Task<Opt<BrandData>> GetBrands()
    {
        if (_cache.BrandData.IsOkay)
            return _cache.BrandData;

        Opt<BrandsDto> fetchResult = await _http.TryGetRequest<BrandsDto>( "Get Brands" );

        if (!fetchResult.IsOkay)
            return Opt<BrandData>.None( fetchResult );

        Dictionary<Guid, Brand> brandsById = [];
        foreach ( Brand b in fetchResult.Data.Brands )
            brandsById.TryAdd( b.Id, b );
        Dictionary<Guid, IReadOnlyList<Brand>> brandsByCategory = [];
        foreach ( KeyValuePair<Guid, List<Guid>> kvp in fetchResult.Data.BrandCategories ) {
            List<Brand> brandsForCategory = [];
            foreach ( Guid brandId in kvp.Value )
                brandsForCategory.Add( brandsById[brandId] );
            brandsByCategory.TryAdd( kvp.Key, brandsForCategory );
        }
        BrandData data = new( brandsById, brandsByCategory );
        _cache.BrandData = Opt<BrandData>.With( data );
        return _cache.BrandData;
    }
}