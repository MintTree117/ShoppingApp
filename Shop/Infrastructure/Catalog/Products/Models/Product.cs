using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Catalog.Products.Dtos;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Products.Models;

public class Product(
    Guid id,
    Brand Brand,
    List<Category> Categories,
    string Name,
    string Image,
    bool IsInStock,
    bool IsFeatured,
    int ShippingDays,
    decimal Price,
    decimal SalePrice,
    int NumberRatings,
    float Rating,
    string? Description,
    string? Xml )
{
    public Guid Id { get; set; } = id;
    public Brand Brand { get; set; } = Brand;
    public List<Category> Categories { get; set; } = Categories;
    public string Name { get; set; } = Name;
    public string Image { get; set; } = Image;
    public bool IsInStock { get; set; } = IsInStock;
    public bool IsFeatured { get; set; } = IsFeatured;
    public int ShippingDays { get; set; } = ShippingDays;
    public decimal Price { get; set; } = Price;
    public decimal SalePrice { get; set; } = SalePrice;
    public int NumberRatings { get; set; } = NumberRatings;
    public float Rating { get; set; } = Rating;
    public string? Description { get; set; } = Description;
    public string? Xml { get; set; } = Xml;
    public static Product From( SearchItemDto dto, int shippingDays, BrandsCollection brands )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );

        return new Product(
            dto.Id,
            (b ? brand : new Brand( Guid.Empty, "None", "None" ))!,
            [],
            dto.Name,
            dto.Image,
            dto.IsInStock,
            dto.IsFeatured,
            shippingDays,
            dto.Price,
            dto.SalePrice,
            dto.NumberRatings,
            dto.Rating,
            null,
            null );
    }

    public static Product From( ProductDto dto, CategoriesCollection categories, BrandsCollection brands )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );
        List<Category> c = [];
        foreach ( Guid cid in dto.CategoryIds )
            if (categories.Categories.TryGetValue( cid, out var category ))
                c.Add( category );
        return new Product(
            dto.Id,
            (b ? brand : new Brand( Guid.Empty, "None", "None" ))!,
            c,
            dto.Name,
            dto.Image,
            dto.IsInStock,
            dto.IsFeatured,
            dto.ShippingDays,
            dto.Price,
            dto.SalePrice,
            dto.NumberRatings,
            dto.Rating,
            dto.Description,
            dto.Xml );
    }
}