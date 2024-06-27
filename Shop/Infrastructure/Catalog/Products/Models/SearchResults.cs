using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Products.Dtos;

namespace Shop.Infrastructure.Catalog.Products.Models;

public class SearchResults(
    int totalMatches,
    List<ProductDetails> results )
{
    public int TotalMatches { get; set; } = totalMatches;
    public List<ProductDetails> Results { get; set; } = results;
    public static SearchResults From( ProductSearchDto search, BrandsCollection brands )
    {
        List<ProductDetails> searchItems = [];
        int maxIndex = search.ShippingDays.Count - 1;
        for ( int i = 0; i < search.Results.Count; i++ )
        {
            ProductSummaryDto dto = search.Results[i];
            int shippingDays = i <= maxIndex ? search.ShippingDays[i] : 0;
            searchItems.Add( ProductDetails.From( dto, shippingDays, brands ) );
        }

        return new SearchResults( search.TotalMatches, searchItems );
    }
    public static SearchResults Empty() =>
        new( 0, [] );
}