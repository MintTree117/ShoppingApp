namespace Shop.Infrastructure.Catalog.Categories.Types;

public readonly record struct CategoryDto(
    Guid Id,
    Guid? ParentId,
    string Name );