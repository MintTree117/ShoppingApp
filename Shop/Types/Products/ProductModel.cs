using Shop.Types.Brands;
using Shop.Types.Categories;
using Shop.Types.Products.Dtos;

namespace Shop.Types.Products;

public sealed class ProductModel(
    Guid id,
    Guid brandId,
    string name,
    string brandName,
    string image,
    bool isFeatured,
    bool isInStock,
    decimal price,
    decimal salePrice,
    float rating,
    int numberRatings,
    int shippingDays,
    List<Category>? categories,
    string? description,
    string? xml )
{
    public Guid Id { get; set; } = id;
    public Guid BrandId { get; set; } = brandId;
    public string BrandName { get; set; } = name; // de-normalized
    public string Name { get; set; } = name;
    public string Image { get; set; } = image;
    public bool IsFeatured { get; set; } = isFeatured;
    public bool IsInStock { get; set; } = isInStock;
    public decimal Price { get; set; } = price;
    public decimal SalePrice { get; set; } = salePrice;
    public float Rating { get; set; } = rating;
    public int NumberRatings { get; set; } = numberRatings;
    public int ShippingDays { get; set; } = shippingDays;
    public List<Category>? Categories { get; set; } = categories;
    public string? Description { get; set; } = description;
    public string? Xml { get; set; } = xml;

    public static List<ProductModel> From( IEnumerable<ProductSummaryDto> dtos )
    {
        List<ProductModel> models = [];
        models.AddRange( from d in dtos select From( d ) );
        return models;
    }
    
    public static ProductModel From( ProductSummaryDto summaryDto )
    {
        return new ProductModel(
            summaryDto.Id,
            summaryDto.BrandId,
            summaryDto.Name,
            summaryDto.BrandName,
            summaryDto.Image,
            summaryDto.IsFeatured,
            summaryDto.IsInStock,
            summaryDto.Price,
            summaryDto.SalePrice,
            summaryDto.Rating,
            summaryDto.NumberRatings,
            summaryDto.ShippingDays,
            null,
            null,
            null );
    }
    public static ProductModel From( ProductDetailsDto detailsDto, CategoriesCollection categories, BrandsCollection brands )
    {
        List<Category> selectedCategories = [];
        if (detailsDto.CategoryIds != null)
            foreach ( Guid cid in detailsDto.CategoryIds )
                if (categories.Categories.TryGetValue( cid, out var category ))
                    selectedCategories.Add( category );
        
        Brand? b = null;
        brands?.BrandsById.TryGetValue( detailsDto.BrandId, out b );

        return new ProductModel(
            detailsDto.Id,
            detailsDto.BrandId,
            detailsDto.Name,
            detailsDto.BrandName,
            detailsDto.Image,
            detailsDto.IsFeatured,
            detailsDto.IsInStock,
            detailsDto.Price,
            detailsDto.SalePrice,
            detailsDto.Rating,
            detailsDto.NumberRatings,
            detailsDto.ShippingDays,
            selectedCategories,
            detailsDto.Description,
            detailsDto.Xml );
    }
}