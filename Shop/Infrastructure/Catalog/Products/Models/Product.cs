using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Products.Models;

public record Product(
    Guid ProductId,
    Brand Brand,
    List<Category> Categories,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    int ShippingDays,
    decimal Price,
    decimal SalePrice,
    string Description,
    string Xml )
{
    public static Product From( SearchItemDto dto, BrandsCollection brands, CategoriesCollection categories )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );
        throw new Exception( "From not implemented on Product." );
    }
}