using Shop.Infrastructure.Catalog.Brands.Types;
using Shop.Infrastructure.Catalog.Categories.Types;
using Shop.Infrastructure.Catalog.Products.Dtos;

namespace Shop.Infrastructure.Catalog.Products.Models;

public class ProductDetails(
    Guid id,
    Brand brand,
    bool isFeatured,
    bool isInStock,
    string name,
    string image,
    decimal price,
    decimal salePrice,
    float rating,
    int numberRatings,
    int shippingDays,
    List<Category> categories,
    string? description,
    string? xml )
{
    public Guid Id { get; set; } = id;
    public Brand Brand { get; set; } = brand;
    public bool IsFeatured { get; set; } = isFeatured;
    public bool IsInStock { get; set; } = isInStock;
    public string Name { get; set; } = name;
    public string Image { get; set; } = image;
    public decimal Price { get; set; } = price;
    public decimal SalePrice { get; set; } = salePrice;
    public float Rating { get; set; } = rating;
    public int NumberRatings { get; set; } = numberRatings;
    public int ShippingDays { get; set; } = shippingDays;
    public List<Category> Categories { get; set; } = categories;
    public string? Description { get; set; } = description;
    public string? Xml { get; set; } = xml;
    
    public static ProductDetails From( ProductSummaryDto summary, int shippingDays, BrandsCollection brands )
    {
        brands.BrandsById.TryGetValue( summary.BrandId, out Brand? brand );
        return new ProductDetails(
            summary.Id,
            brand ?? Brand.Default(),
            summary.IsFeatured,
            summary.IsInStock,
            summary.Name,
            summary.Image,
            summary.Price,
            summary.SalePrice,
            summary.Rating,
            summary.NumberRatings,
            shippingDays,
            [],
            null,
            null );
    }
    public static ProductDetails From( ProductDetailsDto detailsDto, CategoriesCollection categories, BrandsCollection brands )
    {
        List<Category> c = [];
        if (detailsDto.CategoryIds != null)
            foreach ( Guid cid in detailsDto.CategoryIds )
                if (categories.Categories.TryGetValue( cid, out var category ))
                    c.Add( category );
        brands.BrandsById.TryGetValue( detailsDto.BrandId, out Brand? brand );
        
        return new ProductDetails(
            detailsDto.Id,
            brand ?? Brand.Default(),
            detailsDto.IsFeatured,
            detailsDto.IsInStock,
            detailsDto.Name,
            detailsDto.Image,
            detailsDto.Price,
            detailsDto.SalePrice,
            detailsDto.Rating,
            detailsDto.NumberRatings,
            detailsDto.ShippingDays,
            c,
            detailsDto.Description,
            detailsDto.Xml );
    }
}