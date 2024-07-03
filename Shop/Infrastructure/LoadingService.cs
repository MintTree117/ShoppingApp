namespace Shop.Infrastructure;

public sealed class LoadingService
{
    public event Action? OnChangePage;
    bool _isLoadingPage;
    string? _message;

    internal bool IsLoadingPage
    {
        get => _isLoadingPage;
        private set
        {
            if (_isLoadingPage == value) 
                return;
            _isLoadingPage = value;
            NotifyStateChangedPage();
        }
    }
    internal string? Message
    {
        get => _message;
        private set
        {
            if (_message == value) 
                return;
            _message = value;
            NotifyStateChangedPage();
        }
    }
    internal void StartLoadingPage( string? message = null )
    {
        Message = message;
        IsLoadingPage = true;
    }
    internal void StopLoadingPage()
    {
        IsLoadingPage = false;
        Message = null;
    }
    void NotifyStateChangedPage() => 
        OnChangePage?.Invoke();
}