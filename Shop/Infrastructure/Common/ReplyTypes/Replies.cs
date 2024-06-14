namespace Shop.Infrastructure.Common.ReplyTypes;

public readonly record struct Replies<T> : IReply
{
    public static bool operator true( Replies<T> reply ) => reply.Succeeded;
    public static bool operator false( Replies<T> reply ) => !reply.Succeeded;
    public static implicit operator bool( Replies<T> reply ) => reply.Succeeded;
    
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    public readonly bool Succeeded;
    public IEnumerable<T> Enumerable => _enumerable ?? Array.Empty<T>();
    public object GetData() => _enumerable ?? Array.Empty<T>();
    public string GetMessage() => _message ?? string.Empty;
    public bool CheckSuccess() => Succeeded;
    
    public bool OutFailure( out IReply self )
    {
        self = this;
        return !Succeeded;
    }
    public bool OutSuccess( out Replies<T> self )
    {
        self = this;
        return Succeeded;
    }
    public bool OutFailure( out Replies<T> self )
    {
        self = this;
        return !Succeeded;
    }
    
    public static Replies<T> Success( IEnumerable<T> objs ) => new( objs );
    public static Replies<T> Fail() => new();
    public static Replies<T> Fail( string msg ) => new( msg );
    public static Replies<T> Fail( IReply reply ) => new( reply.GetMessage() );
    public static Replies<T> Fail( Exception ex ) => new( ex );
    public static Replies<T> Fail( Exception ex, string msg ) => new( ex, msg );

    public static Replies<T> NotFound() =>
        Fail( MsgNotFound );
    public static Replies<T> NotFound( string msg ) =>
        Fail( $"{MsgNotFound} {msg}" );
    public static Replies<T> NotFound( IReply other ) =>
        Fail( $"{MsgNotFound} {other.GetMessage()}" );

    public static Replies<T> UserNotFound() =>
        Fail( MsgUserNotFound );
    public static Replies<T> UserNotFound( string msg ) =>
        Fail( $"{MsgUserNotFound} {msg}" );
    public static Replies<T> UserNotFound( IReply other ) =>
        Fail( $"{MsgUserNotFound} {other.GetMessage()}" );

    public static Replies<T> Invalid() =>
        Fail( MsgValidationFailure );
    public static Replies<T> Invalid( string msg ) =>
        Fail( $"{MsgValidationFailure} {msg}" );
    public static Replies<T> Invalid( IReply other ) =>
        Fail( $"{MsgValidationFailure} {other.GetMessage()}" );

    public static Replies<T> InvalidPassword() =>
        Fail( MsgPasswordFailure );
    public static Replies<T> InvalidPassword( string msg ) =>
        Fail( $"{MsgPasswordFailure} {msg}" );
    public static Replies<T> InvalidPassword( IReply other ) =>
        Fail( $"{MsgPasswordFailure} {other.GetMessage()}" );

    public static Replies<T> ChangesNotSaved() =>
        Fail( MsgChangesNotSaved );
    public static Replies<T> ChangesNotSaved( string msg ) =>
        Fail( $"{MsgChangesNotSaved} {msg}" );
    public static Replies<T> ChangesNotSaved( IReply other ) =>
        Fail( $"{MsgChangesNotSaved} {other.GetMessage()}" );

    public static Replies<T> Conflict() =>
        Fail( MsgConflictError );
    public static Replies<T> Conflict( string msg ) =>
        Fail( $"{MsgConflictError} {msg}" );
    public static Replies<T> Conflict( IReply other ) =>
        Fail( $"{MsgConflictError} {other.GetMessage()}" );

    public static Replies<T> ServerError() =>
        Fail( MsgServerError );
    public static Replies<T> ServerError( string msg ) =>
        Fail( $"{MsgServerError} {msg}" );
    public static Replies<T> ServerError( IReply other ) =>
        Fail( $"{MsgServerError} {other.GetMessage()}" );

    public static Replies<T> NetworkError() =>
        Fail( MsgNetworkError );
    public static Replies<T> NetworkError( string msg ) =>
        Fail( $"{MsgNetworkError} {msg}" );
    public static Replies<T> NetworkError( IReply other ) =>
        Fail( $"{MsgNetworkError} {other.GetMessage()}" );

    public static Replies<T> Unauthorized() =>
        Fail( MsgUnauthorized );
    public static Replies<T> Unauthorized( string msg ) =>
        Fail( $"{MsgUnauthorized} {msg}" );
    public static Replies<T> Unauthorized( IReply other ) =>
        Fail( $"{MsgUnauthorized} {other.GetMessage()}" );

    public static Replies<T> BadRequest() =>
        Fail( MsgBadRequest );
    public static Replies<T> BadRequest( string msg ) =>
        Fail( $"{MsgBadRequest} {msg}" );
    public static Replies<T> BadRequest( IReply other ) =>
        Fail( $"{MsgBadRequest} {other.GetMessage()}" );

    const string MsgNotFound = "Request not found.";
    const string MsgUserNotFound = "User not found.";
    const string MsgValidationFailure = "Validation failed.";
    const string MsgPasswordFailure = "Invalid password.";
    const string MsgChangesNotSaved = "Failed to save changes to storage.";
    const string MsgConflictError = "A conflict has occured.";
    const string MsgServerError = "An internal server error occured.";
    const string MsgNetworkError = "An network server error occured.";
    const string MsgUnauthorized = "Unauthorized.";
    public const string MsgBadRequest = "Bad Request.";

    Replies( IEnumerable<T>? enumerable )
    {
        _enumerable = enumerable;
        Succeeded = true;
    }
    Replies( string? message = null ) => _message = message;
    Replies( Exception e, string? message = null ) => _message = $"{message} : Exception : {e} : {e.Message}";
}