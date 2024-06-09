using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Catalog.Brands;

public sealed class BrandsCache
{
    public Opt<BrandData> BrandData = Opt<BrandData>.None();
}