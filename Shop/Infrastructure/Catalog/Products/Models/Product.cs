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
    int NumberSold,
    int NumberRatings,
    float Rating,
    string? Description,
    string? Xml )
{
    public static Product From( SearchItemDto dto, int shippingDays, BrandsCollection brands )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );

        return new Product(
            dto.Id,
            (b ? brand : new Brand( Guid.Empty, "None", "None" ))!,
            [],
            dto.Name,
            dto.Image,
            dto.IsFeatured,
            dto.IsInStock,
            shippingDays,
            dto.Price,
            dto.SalePrice,
            dto.NumberSold,
            dto.NumberRatings,
            dto.Rating,
            null,
            null );
    }
}