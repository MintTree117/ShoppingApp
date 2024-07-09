using Shop.Types.Brands;
using Shop.Types.Categories;
using Shop.Types.Products.Dtos;

namespace Shop.Types.Products;

public sealed class ProductModel
{
    // ReSharper disable once UnusedMember.Global
    public ProductModel() { }
    
    public ProductModel(
        Guid id,
        Guid brandId,
        string name,
        string brandName,
        string image,
        bool isFeatured,
        bool isInStock,
        decimal price,
        decimal? salePrice,
        decimal? shippingPrice,
        DateTime? saleEndDate,
        DateTime releaseDate,
        float rating,
        int numberRatings,
        decimal weight,
        string dimensions,
        int shippingDays,
        List<Category>? categories,
        string? description,
        string? xml)
    {
        Id = id;
        BrandId = brandId;
        Name = name;
        BrandName = brandName;
        Image = image;
        IsFeatured = isFeatured;
        IsInStock = isInStock;
        Price = price;
        SalePrice = salePrice;
        ShippingPrice = shippingPrice;
        SaleEndDate = saleEndDate;
        ReleaseDate = releaseDate;
        Rating = rating;
        NumberRatings = numberRatings;
        Weight = weight;
        Dimensions = dimensions;
        ShippingDays = shippingDays;
        Categories = categories;
        Description = description;
        Xml = xml;
    }
    
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty; // de-normalized
    public string Image { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsInStock { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? ShippingPrice { get; set; }
    public DateTime? SaleEndDate { get; set; }
    public DateTime ReleaseDate { get; set; }
    public float Rating { get; set; }
    public int NumberRatings { get; set; }
    public decimal Weight { get; set; }
    public string Dimensions { get; set; } = string.Empty;
    public int ShippingDays { get; set; }
    public List<Category>? Categories { get; set; }
    public string? Description { get; set; }
    public string? Xml { get; set; }

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
            summaryDto.ShippingPrice,
            summaryDto.SaleEndDate,
            summaryDto.ReleaseDate,
            summaryDto.Rating,
            summaryDto.NumberRatings,
            summaryDto.Weight,
            summaryDto.Dimensions,
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
            detailsDto.ShippingPrice,
            detailsDto.SaleEndDate,
            detailsDto.ReleaseDate,
            detailsDto.Rating,
            detailsDto.NumberRatings,
            detailsDto.Weight,
            detailsDto.Dimensions,
            detailsDto.ShippingDays,
            selectedCategories,
            detailsDto.Description,
            detailsDto.Xml );
    }

    public bool IsSaleEndDateNear( out int days )
    {
        days = 0;
        if (SaleEndDate is null)
            return false;

        TimeSpan difference = SaleEndDate.Value.Date - DateTime.Today;
        days = difference.Days;
        return difference.Days <= 30;
    }
}