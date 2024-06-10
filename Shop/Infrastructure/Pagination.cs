namespace Shop.Infrastructure;

public readonly record struct Pagination(
    int Page,
    int PageSize,
    int OrderBy );
    
