namespace Shop.Infrastructure.Common.ReplyTypes;

public readonly record struct Replies<T> : IReply
{
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    public IEnumerable<T> Enumerable => _enumerable ?? Array.Empty<T>();
    
    public string Message() => _message ?? string.Empty;
    public bool IsOkay { get; init; }
    
    public static Replies<T> With( IEnumerable<T> objs ) => new( objs );
    public static Replies<T> False() => new();
    public static Replies<T> False( string msg ) => new( msg );
    public static Replies<T> False( IReply reply ) => new( reply.Message() );
    public static Replies<T> False( IReply reply, string msg ) => new( $"{msg}{reply.Message()} " );
    public static Replies<T> False( Exception ex, string msg ) => new( ex, msg );

    Replies( IEnumerable<T>? enumerable )
    {
        _enumerable = enumerable;
        IsOkay = true;
    }
    Replies( string? message = null ) =>
        _message = message;
    Replies( Exception e, string? message = null ) =>
        _message = $"{message} : Exception : {e} : {e.Message}";
}