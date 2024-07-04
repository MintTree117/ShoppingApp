namespace Shop.Types.Products.Dtos;

public readonly record struct ProductsSimilarDto(
    List<ProductSummaryDto> SimilarToBrand,
    List<ProductSummaryDto> SimilarToProduct );