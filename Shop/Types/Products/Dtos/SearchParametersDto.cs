namespace Shop.Types.Products.Dtos;

public sealed class SearchParametersDto(
    string? searchText,
    Guid? categoryId,
    HashSet<Guid>? brandIds,
    bool? isInStock,
    bool? isFeatured,
    bool? isOnSale,
    bool? isFreeShipping,
    int? minPrice,
    int? maxPrice,
    int page,
    int pageSize,
    int sortBy,
    int? posX,
    int? posY )
{
    public string? SearchText { get; set; } = searchText;
    public Guid? CategoryId { get; set; } = categoryId;
    public HashSet<Guid>? BrandIds { get; set; } = brandIds;
    public bool? IsInStock { get; set; } = isInStock;
    public bool? IsFeatured { get; set; } = isFeatured;
    public bool? IsOnSale { get; set; } = isOnSale;
    public bool? IsFreeShipping { get; set; } = isFreeShipping;
    public int? MinPrice { get; set; } = minPrice;
    public int? MaxPrice { get; set; } = maxPrice;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int SortBy { get; set; } = sortBy;
    public int? PosX { get; set; } = posX;
    public int? PosY { get; set; } = posY;

    public static SearchParametersDto Empty() =>
        new( null, null, null, null, null, null, null, null, null, 1, 5, 0, null, null );
}