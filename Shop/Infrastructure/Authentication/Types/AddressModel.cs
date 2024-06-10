namespace Shop.Infrastructure.Authentication.Types;

public sealed class AddressModel
{
    public Guid Id { get; set; }
    public bool IsPrimary { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
}