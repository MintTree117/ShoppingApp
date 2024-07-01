namespace Shop.Types.Common.ReplyTypes;


public readonly record struct Reply<T> : IReply
{
    public static bool operator true( Reply<T> reply ) => reply.Succeeded;
    public static bool operator false( Reply<T> reply ) => !reply.Succeeded;
    public static implicit operator bool( Reply<T> reply ) => reply.Succeeded;

    // Intentionally Unsafe: Up to programmer to keep track
    public readonly bool Succeeded;
    public T Data => _obj ?? throw new Exception( $"!!!!!!!!!!!! Fatal: Reply<{typeof( T )}>: Tried to access a null reply. !!!!!!!!!!!!" );
    public object GetData() => _obj ?? throw new Exception( $"!!!!!!!!!!!! Fatal: Reply<{typeof( T )}>: Tried to access a null reply. !!!!!!!!!!!!" );
    public string GetMessage() => _message ?? string.Empty;
    public bool CheckSuccess() => Succeeded;

    readonly T? _obj = default;
    readonly string? _message = null;
    
    public bool OutSuccess( out Reply<T> self )
    {
        self = this;
        return Succeeded;
    }
    public bool OutFailure( out Reply<T> self )
    {
        self = this;
        return !Succeeded;
    }
    
    public static Reply<T> Success( T obj ) => new( obj );
    public static Reply<T> Fail() => new();
    public static Reply<T> Fail( string msg ) => new( msg );
    public static Reply<T> Fail( IReply reply ) => new( reply.GetMessage() );
    public static Reply<T> Fail( IReply reply, string msg ) => new( $"{msg} {reply.GetMessage()}" );
    public static Reply<T> Fail( Exception ex ) => new( ex );
    public static Reply<T> Fail( Exception ex, string msg ) => new( ex, msg );

    public static Reply<T> NotFound() =>
        Fail( MsgNotFound );
    public static Reply<T> NotFound( string msg ) =>
        Fail( $"{MsgNotFound} {msg}" );
    public static Reply<T> NotFound( IReply other ) =>
        Fail( $"{MsgNotFound} {other.GetMessage()}" );

    public static Reply<T> UserNotFound() =>
        Fail( MsgUserNotFound );
    public static Reply<T> UserNotFound( string msg ) =>
        Fail( $"{MsgUserNotFound} {msg}" );
    public static Reply<T> UserNotFound( IReply other ) =>
        Fail( $"{MsgUserNotFound} {other.GetMessage()}" );

    public static Reply<T> Invalid() =>
        Fail( MsgValidationFailure );
    public static Reply<T> Invalid( string msg ) =>
        Fail( $"{MsgValidationFailure} {msg}" );
    public static Reply<T> Invalid( IReply other ) =>
        Fail( $"{MsgValidationFailure} {other.GetMessage()}" );

    public static Reply<T> InvalidPassword() =>
        Fail( MsgPasswordFailure );
    public static Reply<T> InvalidPassword( string msg ) =>
        Fail( $"{MsgPasswordFailure} {msg}" );
    public static Reply<T> InvalidPassword( IReply other ) =>
        Fail( $"{MsgPasswordFailure} {other.GetMessage()}" );

    public static Reply<T> ChangesNotSaved() =>
        Fail( MsgChangesNotSaved );
    public static Reply<T> ChangesNotSaved( string msg ) =>
        Fail( $"{MsgChangesNotSaved} {msg}" );
    public static Reply<T> ChangesNotSaved( IReply other ) =>
        Fail( $"{MsgChangesNotSaved} {other.GetMessage()}" );

    public static Reply<T> Conflict() =>
        Fail( MsgConflictError );
    public static Reply<T> Conflict( string msg ) =>
        Fail( $"{MsgConflictError} {msg}" );
    public static Reply<T> Conflict( IReply other ) =>
        Fail( $"{MsgConflictError} {other.GetMessage()}" );

    public static Reply<T> StorageError() =>
        Fail( MsgStorageError );
    public static Reply<T> StorageError( string msg ) =>
        Fail( $"{MsgStorageError} {msg}" );
    public static Reply<T> StorageError( IReply other ) =>
        Fail( $"{MsgStorageError} {other.GetMessage()}" );
    
    public static Reply<T> ServerError() =>
        Fail( MsgServerError );
    public static Reply<T> ServerError( string msg ) =>
        Fail( $"{MsgServerError} {msg}" );
    public static Reply<T> ServerError( IReply other ) =>
        Fail( $"{MsgServerError} {other.GetMessage()}" );

    public static Reply<T> NetworkError() =>
        Fail( MsgNetworkError );
    public static Reply<T> NetworkError( string msg ) =>
        Fail( $"{MsgNetworkError} {msg}" );
    public static Reply<T> NetworkError( IReply other ) =>
        Fail( $"{MsgNetworkError} {other.GetMessage()}" );

    public static Reply<T> Unauthorized() =>
        Fail( MsgUnauthorized );
    public static Reply<T> Unauthorized( string msg ) =>
        Fail( $"{MsgUnauthorized} {msg}" );
    public static Reply<T> Unauthorized( IReply other ) =>
        Fail( $"{MsgUnauthorized} {other.GetMessage()}" );
    
    public static Reply<T> BadRequest() =>
        Fail( MsgBadRequest );
    public static Reply<T> BadRequest( string msg ) =>
        Fail( $"{MsgBadRequest} {msg}" );
    public static Reply<T> BadRequest( IReply other ) =>
        Fail( $"{MsgBadRequest} {other.GetMessage()}" );
    
    
    const string MsgNotFound = "Request not found.";
    const string MsgUserNotFound = "User not found.";
    const string MsgValidationFailure = "Validation failed.";
    const string MsgPasswordFailure = "Invalid password.";
    const string MsgChangesNotSaved = "Failed to save changes to storage.";
    const string MsgConflictError = "A conflict has occured.";
    const string MsgStorageError = "A storage error has occured.";
    const string MsgServerError = "An internal server error occured.";
    const string MsgNetworkError = "An network server error occured.";
    const string MsgUnauthorized = "Unauthorized.";
    const string MsgBadRequest = "Bad Request.";

    Reply( T obj )
    {
        _obj = obj;
        Succeeded = true;
    }
    Reply( string? message = null ) =>
        _message = message;
    Reply( Exception e, string? message = null ) => 
        _message = $"{message} : Exception : {e} : {e.Message}";
}