namespace ShopApplication.Common.Optionals;

public interface IOpt
{
    public string Message();
    public bool IsOkay();

    public static Opt<bool> Okay() => Opt<bool>.With( true );
    public static Opt<bool> None( string message ) => Opt<bool>.None( message );
    public static Opt<bool> None( IOpt other ) => Opt<bool>.None( other );
}