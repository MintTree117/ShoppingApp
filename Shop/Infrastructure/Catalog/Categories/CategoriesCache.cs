using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure.Catalog.Categories;

public sealed class CategoriesCache( StorageService storage ) : 
    MemoryCache<CategoriesCollection>( "Categories", storage, TimeSpan.FromHours( 24 ) );