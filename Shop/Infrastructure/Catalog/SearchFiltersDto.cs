namespace Shop.Infrastructure.Catalog;

public readonly record struct SearchFiltersDto(
    Guid? CategoryId,
    HashSet<Guid> BrandIds,
    int? MinPrice,
    int? MaxPrice,
    bool? IsInStock,
    bool? IsFeatured,
    bool IsOnSale );