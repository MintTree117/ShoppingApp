namespace Shop.Types.Users;

public sealed class AddressModel
{
    public AddressModel()
    {
        
    }
    public AddressModel( Guid id, string name, int posX, int posY )
    {
        Id = id;
        Name = name;
        PosX = posX;
        PosY = posY;
    }

    public Guid Id { get; set; }
    public bool IsPrimary { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PosX { get; set; }
    public int PosY { get; set; }
}