using Shop.Types.Products.Dtos;

namespace Shop.Types.Cart;

public sealed class CartItemsAndSummaries
{
    CartItemsAndSummaries( Dictionary<CartItemDto, ProductSummaryDto> newItemsAndSummaries ) => 
        _itemsAndSummaries = newItemsAndSummaries;

    readonly Dictionary<CartItemDto, ProductSummaryDto> _itemsAndSummaries;

    public int Count() =>
        _itemsAndSummaries.Keys.Count;
    public decimal Price()
    {
        decimal totalPrice = decimal.Zero;
        foreach ( var kvp in _itemsAndSummaries )
        {
            int count = kvp.Key.Quantity;
            decimal price = kvp.Value.RealPrice();
            totalPrice += count * price;
        }
        return Math.Round( totalPrice, 2 );
    }
    public decimal Savings()
    {
        decimal totalSavings = 0;
        foreach ( var kvp in _itemsAndSummaries )
        {
            if (kvp.Value.SalePrice is null)
                continue;
            
            decimal salePrice = kvp.Value.SalePrice ?? decimal.Zero;
            decimal price = kvp.Value.Price;
            decimal difference = price - salePrice;
            totalSavings += difference;
        }
        return Math.Round( totalSavings, 2 );
    }
    public decimal Shipping()
    {
        decimal totalShipping = 0;
        foreach ( var kvp in _itemsAndSummaries )
        {
            decimal shipping = kvp.Value.ShippingPrice ?? decimal.Zero;
            totalShipping += shipping;
        }
        return Math.Round( totalShipping, 2 );
    }
    public decimal EstimatedTax()
    {
        decimal price = Price();
        decimal tax = price * (decimal) 0.13;
        return Math.Round( tax, 2 );
    }
    public decimal EstimatedTotal()
    {
        decimal price = Price();
        decimal tax = EstimatedTax();
        decimal total = price + tax;
        return Math.Round( total, 2 );
    }
    
    public IEnumerable<CartItemDto> Items() =>
        _itemsAndSummaries.Keys;
    public ProductSummaryDto Dto( CartItemDto itemDto ) =>
        _itemsAndSummaries[itemDto];
    
    public static CartItemsAndSummaries Empty() =>
        new( [] );
    public static CartItemsAndSummaries From( List<CartItemDto> items, List<ProductSummaryDto> dtos )
    {
        Dictionary<CartItemDto, ProductSummaryDto> newItemsAndSummaries = [];
        
        foreach ( CartItemDto c in items )
        {
            var dto = dtos.FirstOrDefault( d => d.Id == c.ProductId );
            newItemsAndSummaries.TryAdd( c, dto );
        }

        return new CartItemsAndSummaries( newItemsAndSummaries );
    } 
}