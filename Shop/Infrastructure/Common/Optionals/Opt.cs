namespace Shop.Infrastructure.Common.Optionals;

public readonly record struct Opt<T> : IOpt
{
    readonly T? _obj = default;
    readonly bool _success = false;
    readonly string? _message = null;
    
    // Intentionally Unsafe: Up to programmer to keep track
    public T Data => _obj ?? Activator.CreateInstance<T>(); // throw new Exception( "Fatal: Tried to access a null Optional!" );
    
    public string Message() => _message ?? string.Empty;
    public bool IsOkay() => _obj is not null && _success;

    public bool Okay( out IOpt self )
    {
        self = this;
        return IsOkay();
    }
    public bool Fail( out IOpt self )
    {
        self = this;
        return !IsOkay();
    }
    public bool Okay( out Opt<T> self )
    {
        self = this;
        return IsOkay();
    }
    public bool Fail( out Opt<T> self )
    {
        self = this;
        return !IsOkay();
    }
    
    public static Opt<T> With( T obj ) => new( obj );
    public static Opt<T> Maybe( T? obj ) => new( obj );
    public static Opt<T> None() => new();
    public static Opt<T> None( string msg ) => new( msg );
    public static Opt<T> None( IOpt opt ) => new( opt.Message() );
    public static Opt<T> Exception( Exception ex ) => new( ex );
    public static Opt<T> Exception( Exception ex, string msg ) => new( ex, msg );

    Opt( T? obj ) => _obj = obj;
    Opt( string? message = null ) => _message = message;
    Opt( Exception e, string? message = null ) => _message = $"{message} : Exception : {e} : {e.Message}";
}