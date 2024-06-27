namespace Shop.Infrastructure.Catalog.Products.Models;

public class SearchFilters(
    Guid? CategoryId,
    HashSet<Guid>? BrandIds,
    bool? IsInStock,
    bool? IsFeatured,
    bool? IsOnSale,
    int? MinPrice,
    int? MaxPrice )
{
    public Guid? CategoryId { get; set; } = CategoryId;
    public HashSet<Guid>? BrandIds { get; set; } = BrandIds;
    public bool? IsInStock { get; set; } = IsInStock;
    public bool? IsFeatured { get; set; } = IsFeatured;
    public bool? IsOnSale { get; set; } = IsOnSale;
    public int? MinPrice { get; set; } = MinPrice;
    public int? MaxPrice { get; set; } = MaxPrice;

    public static SearchFilters Empty() =>
        new( null, null, null, null, null, null, null );
}