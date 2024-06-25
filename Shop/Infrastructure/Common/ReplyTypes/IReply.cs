namespace Shop.Infrastructure.Common.ReplyTypes;

public interface IReply
{
    public bool CheckSuccess();
    public string GetMessage();
    public object GetData();

    public static Reply<bool> Success() => Reply<bool>.Success( true );
    public static Reply<bool> Fail() => Reply<bool>.Fail();
    public static Reply<bool> Fail( string msg ) => Reply<bool>.Fail( msg );
    public static Reply<bool> Fail( IReply other ) => Reply<bool>.Fail( other );
    public static Reply<bool> Fail( IReply other, string msg ) => Reply<bool>.Fail( other, msg );
    public static Reply<bool> Fail( Exception ex ) => Reply<bool>.Fail( ex );
    public static Reply<bool> Fail( Exception ex, string msg ) => Reply<bool>.Fail( ex, msg );

    public bool OutSuccess( out IReply self )
    {
        self = this;
        return CheckSuccess();
    }
    public bool OutFailure( out IReply self )
    {
        self = this;
        return !CheckSuccess();
    }

    public static Reply<bool> NotFound() =>
        Reply<bool>.Fail( MsgNotFound );
    public static Reply<bool> NotFound(string msg) =>
        Reply<bool>.Fail($"{MsgNotFound} {msg}");
    public static Reply<bool> NotFound(IReply other) =>
        Reply<bool>.Fail($"{MsgNotFound} {other.GetMessage()}");

    public static Reply<bool> UserNotFound() =>
        Reply<bool>.Fail( MsgUserNotFound );
    public static Reply<bool> UserNotFound(string msg) =>
        Reply<bool>.Fail($"{MsgUserNotFound} {msg}");
    public static Reply<bool> UserNotFound(IReply other) =>
        Reply<bool>.Fail($"{MsgUserNotFound} {other.GetMessage()}");

    public static Reply<bool> Invalid() =>
        Reply<bool>.Fail( MsgValidationFailure );
    public static Reply<bool> Invalid(string msg) =>
        Reply<bool>.Fail($"{MsgValidationFailure} {msg}");
    public static Reply<bool> Invalid(IReply other) =>
        Reply<bool>.Fail($"{MsgValidationFailure} {other.GetMessage()}");

    public static Reply<bool> InvalidPassword() =>
        Reply<bool>.Fail( MsgPasswordFailure );
    public static Reply<bool> InvalidPassword(string msg) =>
        Reply<bool>.Fail($"{MsgPasswordFailure} {msg}");
    public static Reply<bool> InvalidPassword(IReply other) =>
        Reply<bool>.Fail($"{MsgPasswordFailure} {other.GetMessage()}");

    public static Reply<bool> ChangesNotSaved() =>
        Reply<bool>.Fail( MsgChangesNotSaved );
    public static Reply<bool> ChangesNotSaved(string msg) =>
        Reply<bool>.Fail($"{MsgChangesNotSaved} {msg}");
    public static Reply<bool> ChangesNotSaved(IReply other) =>
        Reply<bool>.Fail($"{MsgChangesNotSaved} {other.GetMessage()}");

    public static Reply<bool> Conflict() =>
        Reply<bool>.Fail( MsgConflictError );
    public static Reply<bool> Conflict(string msg) =>
        Reply<bool>.Fail($"{MsgConflictError} {msg}");
    public static Reply<bool> Conflict(IReply other) =>
        Reply<bool>.Fail($"{MsgConflictError} {other.GetMessage()}");

    public static Reply<bool> StorageError() =>
        Fail( MsgStorageError );
    public static Reply<bool> StorageError( string msg ) =>
        Fail( $"{MsgStorageError} {msg}" );
    public static Reply<bool> StorageError( IReply other ) =>
        Fail( $"{MsgStorageError} {other.GetMessage()}" );
    
    public static Reply<bool> ServerError() =>
        Reply<bool>.Fail( MsgServerError );
    public static Reply<bool> ServerError(string msg) =>
        Reply<bool>.Fail($"{MsgServerError} {msg}");
    public static Reply<bool> ServerError(IReply other) =>
        Reply<bool>.Fail($"{MsgServerError} {other.GetMessage()}");

    public static Reply<bool> NetworkError() =>
        Fail( MsgNetworkError );
    public static Reply<bool> NetworkError( string msg ) =>
        Fail( $"{MsgNetworkError} {msg}" );
    public static Reply<bool> NetworkError( IReply other ) =>
        Fail( $"{MsgNetworkError} {other.GetMessage()}" );
        
    public static Reply<bool> Unauthorized() =>
        Fail( MsgUnauthorized );
    public static Reply<bool> Unauthorized( string msg ) =>
        Fail( $"{MsgUnauthorized} {msg}" );
    public static Reply<bool> Unauthorized( IReply other ) =>
        Fail( $"{MsgUnauthorized} {other.GetMessage()}" );

    public static Reply<bool> BadRequest() =>
        Fail( MsgBadRequest );
    public static Reply<bool> BadRequest( string msg ) =>
        Fail( $"{MsgBadRequest} {msg}" );
    public static Reply<bool> BadRequest( IReply other ) =>
        Fail( $"{MsgBadRequest} {other.GetMessage()}" );
    
    public const string MsgUnauthorized = "Unauthorized.";
    public const string MsgNotFound = "Request not found.";
    public const string MsgUserNotFound = "User not found.";
    public const string MsgValidationFailure = "Validation failed.";
    public const string MsgPasswordFailure = "Invalid password.";
    public const string MsgChangesNotSaved = "Failed to save changes to storage.";
    public const string MsgConflictError = "A conflict has occured.";
    public const string MsgStorageError = "A storage error has occured.";
    public const string MsgServerError = "An internal server error occured.";
    public const string MsgNetworkError = "An network server error occured.";
    public const string MsgBadRequest = "Bad Request.";
}