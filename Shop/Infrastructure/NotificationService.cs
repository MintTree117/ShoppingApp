namespace Shop.Infrastructure;

public sealed class NotificationService
{
    public event Action<string>? OnPushSuccess;
    public event Action<string>? OnPushError;

    public void PushSuccess( string message )
    {
        OnPushSuccess?.Invoke( message );
    }
    public void PushError( string message )
    {
        OnPushError?.Invoke( message );
    }
}