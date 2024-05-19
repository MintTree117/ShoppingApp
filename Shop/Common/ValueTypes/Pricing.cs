namespace ShopApplication.Common.ValueTypes;

public readonly record struct Pricing(
    string PromoCode,
    decimal Subtotal,
    decimal ItemDiscount,
    decimal Tax,
    decimal ShippingCharges,
    decimal TotalPrice, 
    decimal TotalDiscount,
    decimal GrandTotal );