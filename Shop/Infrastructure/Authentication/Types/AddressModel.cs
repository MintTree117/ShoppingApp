namespace Shop.Infrastructure.Authentication.Types;

public sealed class AddressModel
{
    public Guid Id { get; set; }
    public bool IsPrimary { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int GridX { get; set; }
    public int GridY { get; set; }
}