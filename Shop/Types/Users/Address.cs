namespace Shop.Types.Users;

public sealed class Address
{
    public string Name { get; set; } = string.Empty;
    public int PosX { get; set; }
    public int PosY { get; set; }

    public Address() { }
    
    public Address( string name, int posX, int posY )
    {
        Name = name;
        PosX = posX;
        PosY = posY;
    }

    public AddressModel ToModel() =>
        new() {
            Name = Name,
            PosX = PosX,
            PosY = PosY
        };

    public static Address? FromModel( AddressModel? m ) =>
        m is null ? null
        : new Address {
            Name = m.Name,
            PosX = m.PosX,
            PosY = m.PosY
        };
}