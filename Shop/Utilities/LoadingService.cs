namespace Shop.Utilities;

public sealed class LoadingService
{
    public event Action? OnChange;
    bool _isLoading;
    string? _message;

    public bool IsLoading
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

    public string? Message
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

    public void StartLoading( string? message = null )
    {
        Message = message;
        IsLoading = true;
    }

    public void StopLoading()
    {
        IsLoading = false;
        Message = null;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}