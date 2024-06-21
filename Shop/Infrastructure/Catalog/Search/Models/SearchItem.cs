using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Search.Models;

public record SearchItem(
    Guid ProductId,
    Brand Brand,
    string Name,
    string Image,
    bool IsFeatured,
    bool IsInStock,
    int ShippingDays,
    decimal Price,
    decimal SalePrice,
    int NumberSold,
    int NumberRatings,
    float Rating )
{
    public static SearchItem From( SearchItemDto dto, int shippingDays, BrandsCollection brands )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );

        return new SearchItem(
            dto.ProductId,
            (b ? brand : new Brand( Guid.Empty, "None", "None" ))!,
            dto.Name,
            dto.Image,
            dto.IsFeatured,
            dto.IsInStock,
            shippingDays,
            dto.Price,
            dto.SalePrice,
            dto.NumberSold,
            dto.NumberRatings,
            dto.Rating );
    }
}