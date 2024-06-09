using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsCache( StorageService storage ) : 
    MemoryCache<BrandsCollection>( "Brands", storage, TimeSpan.FromHours( 24 ) );