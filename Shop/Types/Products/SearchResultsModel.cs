using Shop.Types.Products.Dtos;

namespace Shop.Types.Products;

public sealed class SearchResultsModel
{
    public int TotalCount { get; set; }
    public List<ProductModel> Products { get; set; } = [];

    public static SearchResultsModel Empty() =>
        new() {
            TotalCount = 0,
            Products = []
        };
    public static SearchResultsModel From( SearchResultsDto dto ) =>
        new() {
            TotalCount = dto.TotalMatches,
            Products = ProductModel.From( dto.Results )
        };
}