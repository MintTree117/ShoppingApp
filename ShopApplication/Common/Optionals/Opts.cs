namespace RetailDomain.Optionals;

public readonly record struct Opts<T> : IOpt
{
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    public IEnumerable<T> Enumerable => _enumerable ?? Array.Empty<T>();
    
    public string Message() => _message ?? string.Empty;
    public bool IsOkay() => _enumerable is not null;

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
    public bool Okay( out Opts<T> self )
    {
        self = this;
        return IsOkay();
    }
    public bool Fail( out Opts<T> self )
    {
        self = this;
        return !IsOkay();
    }
    
    public static Opts<T> With( IEnumerable<T> objs ) => new( objs );
    public static Opts<T> Maybe( IEnumerable<T>? objs ) => new( objs );
    public static Opts<T> None() => new();
    public static Opts<T> Error( string msg ) => new( msg );
    public static Opts<T> None( IOpt opt ) => new( opt.Message() );
    public static Opts<T> Exception( Exception ex ) => new( ex );
    public static Opts<T> Exception( Exception ex, string msg ) => new( ex, msg );

    Opts( IEnumerable<T>? enumerable ) => _enumerable = enumerable;
    Opts( string? message = null ) => _message = message;
    Opts( Exception e, string? message = null ) => _message = $"{message} : Exception : {e} : {e.Message}";
}