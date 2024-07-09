namespace Shop.Types.Orders;

public enum OrderState
{
    Placed,
    Fulfilling,
    Shipping,
    Delivered,
    Cancelled
}