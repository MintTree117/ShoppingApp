namespace Shop.Infrastructure.Catalog;

public readonly record struct SearchFiltersDto(
    HashSet<Guid> CategoryIds,
    HashSet<Guid> BrandIds,
    int? MinPrice,
    int? MaxPrice,
    bool? IsInStock,
    bool? IsFeatured,
    bool IsOnSale );