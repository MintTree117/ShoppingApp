namespace Shop.Infrastructure.Catalog;

public sealed class Category
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public int Tier { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Category> Children { get; set; } = [];
}