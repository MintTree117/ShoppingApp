namespace Shop.Infrastructure.Common.Optionals;

public readonly record struct Replies<T> : IReply
{
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    public IEnumerable<T> Enumerable => _enumerable ?? Array.Empty<T>();
    
    public string Message() => _message ?? string.Empty;
    public bool IsOkay { get; init; }

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
    public bool Okay( out Replies<T> self )
    {
        self = this;
        return IsOkay;
    }
    public bool Fail( out Replies<T> self )
    {
        self = this;
        return !IsOkay;
    }
    
    public static Replies<T> With( IEnumerable<T> objs ) => new( objs );
    public static Replies<T> Maybe( IEnumerable<T>? objs ) => new( objs );
    public static Replies<T> None() => new();
    public static Replies<T> Error( string msg ) => new( msg );
    public static Replies<T> None( IReply reply ) => new( reply.Message() );
    public static Replies<T> Exception( Exception ex ) => new( ex );
    public static Replies<T> Exception( Exception ex, string msg ) => new( ex, msg );

    Replies( IEnumerable<T>? enumerable )
    {
        _enumerable = enumerable;
        IsOkay = true;
    }
    Replies( string? message = null ) => _message = message;
    Replies( Exception e, string? message = null ) => _message = $"{message} : Exception : {e} : {e.Message}";
}