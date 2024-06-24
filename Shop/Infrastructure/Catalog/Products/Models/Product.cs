using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Catalog.Products.Dtos;
using Shop.Infrastructure.Catalog.Search.Dtos;

namespace Shop.Infrastructure.Catalog.Products.Models;

public class Product(
    Guid id,
    Brand brand,
    List<Category> categories,
    string name,
    string image,
    bool isFeatured,
    bool isInStock,
    int shippingDays,
    decimal price,
    decimal salePrice,
    int numberRatings,
    float rating,
    string? description,
    string? xml )
{
    public Guid Id { get; set; } = id;
    public Brand Brand { get; set; } = brand;
    public List<Category> Categories { get; set; } = categories;
    public string Name { get; set; } = name;
    public string Image { get; set; } = image;
    public bool IsFeatured { get; set; } = isFeatured;
    public bool IsInStock { get; set; } = isInStock;
    public int ShippingDays { get; set; } = shippingDays;
    public decimal Price { get; set; } = price;
    public decimal SalePrice { get; set; } = salePrice;
    public int NumberRatings { get; set; } = numberRatings;
    public float Rating { get; set; } = rating;
    public string? Description { get; set; } = description;
    public string? Xml { get; set; } = xml;
    
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
            dto.NumberRatings,
            dto.Rating,
            null,
            null );
    }
    public static Product From( ProductDto dto, CategoriesCollection categories, BrandsCollection brands )
    {
        bool b = brands.BrandsById.TryGetValue( dto.BrandId, out Brand? brand );
        List<Category> c = [];
        
        if (dto.CategoryIds != null)
            foreach ( Guid cid in dto.CategoryIds )
                if (categories.Categories.TryGetValue( cid, out var category ))
                    c.Add( category );
        
        return new Product(
            dto.Id,
            (b ? brand : new Brand( Guid.Empty, "None", "None" ))!,
            c,
            dto.Name,
            dto.Image,
            dto.IsFeatured,
            dto.IsInStock,
            dto.ShippingDays,
            dto.Price,
            dto.SalePrice,
            dto.NumberRatings,
            dto.Rating,
            dto.Description,
            dto.Xml );
    }
}