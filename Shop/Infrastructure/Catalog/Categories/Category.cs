namespace Shop.Infrastructure.Catalog.Categories;

public record Category
{
    public Guid Id { get; init; }
    public Guid? ParentId { get; init; }
    public int Tier { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<Category> Children { get; init; } = [];
}