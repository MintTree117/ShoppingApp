namespace Shop.Infrastructure.Common;

public sealed class LoadingService
{
    public event Action? OnChange;
    bool _isLoading;
    string? _message;

    internal bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value) {
                _isLoading = value;
                NotifyStateChanged();
            }
        }
    }
    internal string? Message
    {
        get => _message;
        private set
        {
            if (_message != value) {
                _message = value;
                NotifyStateChanged();
            }
        }
    }
    internal void StartLoading( string? message = null )
    {
        Message = message;
        IsLoading = true;
    }
    internal void StopLoading()
    {
        IsLoading = false;
        Message = null;
    }

    void NotifyStateChanged() => OnChange?.Invoke();
}