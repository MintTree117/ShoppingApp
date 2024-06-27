namespace Shop.Infrastructure.Catalog.Products.Dtos;

public readonly record struct ProductSuggestionDto(
    Guid Id,
    string Name );