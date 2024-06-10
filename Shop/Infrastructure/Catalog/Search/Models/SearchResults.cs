using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Search.Models;

public record SearchResults(
    int TotalMatches,
    List<SearchItem> Results )
{
    public static SearchResults From( SearchResultsDto resultsDto, BrandsCollection brands )
    {
        List<SearchItem> searchItems = [];
        int maxIndex = resultsDto.ShippingEstimates.Count - 1;
        for ( int i = 0; i < resultsDto.Results.Count; i++ )
        {
            SearchItemDto dto = resultsDto.Results[i];
            int shippingDays = i <= maxIndex ? resultsDto.ShippingEstimates[i] : 0;
            searchItems.Add( SearchItem.From( dto, shippingDays, brands ) );
        }

        return new SearchResults( resultsDto.TotalMatches, searchItems );
    }
}