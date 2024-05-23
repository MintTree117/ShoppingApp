using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Catalog;

public sealed class CategoriesCache
{
    public Opt<CategoryData> categories = Opt<CategoryData>.None();
}