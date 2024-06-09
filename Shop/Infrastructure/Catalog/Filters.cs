using Shop.Infrastructure.Catalog.Brands;
using Shop.Infrastructure.Catalog.Categories;

namespace Shop.Infrastructure.Catalog;

public record Filters(
    List<Category> Categories,
    List<Brand> Brands );