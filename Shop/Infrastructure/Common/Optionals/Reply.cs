namespace Shop.Infrastructure.Common.Optionals;

public readonly record struct Reply<T> : IReply
{
    readonly T? _obj = default;
    readonly string? _message = null;
    
    // Intentionally Unsafe: Up to programmer to keep track
    public T Data => _obj ?? Activator.CreateInstance<T>(); // throw new Exception( "Fatal: Tried to access a null Optional!" );
    public bool IsOkay { get; init; }
    public string Message() => _message ?? string.Empty;

    public bool Okay( out IReply self )
    {
        self = this;
        return IsOkay;
    }
    public bool Fail( out IReply self )
    {
        self = this;
        return !IsOkay;
    }
    public bool Okay( out Reply<T> self )
    {
        self = this;
        return IsOkay;
    }
    public bool Fail( out Reply<T> self )
    {
        self = this;
        return !IsOkay;
    }
    
    public static Reply<T> With( T obj ) => new( obj );
    public static Reply<T> Maybe( T? obj ) => new( obj );
    public static Reply<T> None() => new();
    public static Reply<T> None( string msg ) => new( msg );
    public static Reply<T> None( IReply reply ) => new( reply.Message() );
    public static Reply<T> Exception( Exception ex ) => new( ex );
    public static Reply<T> Exception( Exception ex, string msg ) => new( ex, msg );

    Reply( T obj )
    {
        _obj = obj;
        IsOkay = true;
    }
    Reply( string? message = null ) => _message = message;
    Reply( Exception e, string? message = null ) => _message = $"{message} : Exception : {e} : {e.Message}";
}