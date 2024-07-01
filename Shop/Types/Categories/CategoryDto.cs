namespace Shop.Types.Categories;

public readonly record struct CategoryDto(
    Guid Id,
    Guid? ParentId,
    string Name );