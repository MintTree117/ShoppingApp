using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Products.Models;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Search.Models;

public record SearchResults(
    int TotalMatches,
    List<Product> Results )
{
    public static SearchResults From( SearchResultsDto resultsDto, BrandsCollection brands )
    {
        List<Product> searchItems = [];
        int maxIndex = resultsDto.ShippingDays.Count - 1;
        for ( int i = 0; i < resultsDto.Results.Count; i++ )
        {
            SearchItemDto dto = resultsDto.Results[i];
            int shippingDays = i <= maxIndex ? resultsDto.ShippingDays[i] : 0;
            searchItems.Add( Product.From( dto, shippingDays, brands ) );
        }

        return new SearchResults( resultsDto.TotalMatches, searchItems );
    }
    public static SearchResults Empty() =>
        new( 0, [] );
}