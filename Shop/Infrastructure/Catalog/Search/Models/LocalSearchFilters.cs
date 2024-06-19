namespace Shop.Infrastructure.Catalog.Search.Models;

public class LocalSearchFilters(
    Guid? categoryId,
    HashSet<Guid>? brandIds,
    bool? isInStock,
    bool? isFeatured,
    bool? isOnSale,
    int? minPrice,
    int? maxPrice,
    int page,
    int pageSize,
    int sortBy )
{
    public Guid? CategoryId { get; set; } = categoryId;
    public HashSet<Guid>? BrandIds { get; set; } = brandIds;
    public bool? IsInStock { get; set; } = isInStock;
    public bool? IsFeatured { get; set; } = isFeatured;
    public bool? IsOnSale { get; set; } = isOnSale;
    public int? MinPrice { get; set; } = minPrice;
    public int? MaxPrice { get; set; } = maxPrice;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int SortBy { get; set; } = sortBy;
    
    public static LocalSearchFilters Empty() =>
        new( null, null, null, null, null, null, null, 1, 5, 0 );
}