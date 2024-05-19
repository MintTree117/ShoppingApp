namespace ShopApplication.Common.ValueTypes;

public readonly record struct Contact( 
    int Id, 
    string Phone, 
    string Email, 
    string Name )
{
    public static Contact Empty() => 
        new( -1, string.Empty, string.Empty, string.Empty );
}