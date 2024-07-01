namespace Shop.Types.Common.ValueTypes;

public sealed class Location
{
    public int PosX { get; set; }
    public int PosY { get; set; }

    public static Location With( int x, int y )
    {
        return new Location() {
            PosX = x ,
            PosY = y
        };
    }
}