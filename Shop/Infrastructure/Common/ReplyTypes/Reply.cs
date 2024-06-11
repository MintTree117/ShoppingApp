namespace Shop.Infrastructure.Common.ReplyTypes;

public readonly record struct Reply<T> : IReply
{
    // REPLY DATA
    readonly T? _obj = default;
    readonly string? _message = null;

    // ACCESSORS
    public T Data => _obj ?? throw new Exception( "FATAL: Tried to access a null Optional!" ); // Intentionally Unsafe: Up to programmer to keep track
    public bool IsOkay { get; init; }
    public string Message() => _message ?? string.Empty;

    // SUCCESS
    public static Reply<T> True( T obj ) => new( obj );

    // FAILURE
    public static Reply<T> False() => new();
    public static Reply<T> False( string msg ) => new( msg );
    public static Reply<T> False( IReply reply, string msg ) => new( $"{msg}{reply.Message()} " );
    public static Reply<T> False( IReply reply ) => new( reply.Message() );
    public static Reply<T> False( Exception ex, string msg) => new( $"{msg}{ex}" );
    
    // PRIVATE CONSTRUCTORS
    Reply( T obj )
    {
        _obj = obj;
        IsOkay = true;
    }
    Reply( string? message = null ) => 
        _message = message;
    Reply( Exception e, string? message = null ) => 
        _message = $"{message} : Exception : {e} : {e.Message}";
}