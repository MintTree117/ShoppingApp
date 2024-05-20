using Shop.UI.Shared;

namespace Shop.Utilities;

public class PageState( Action<string> startLoading, Action stopLoading )
{
    public Action<string> StartRedirecting { get; set; } = default!;
    public Action<string> StartLoading { get; set; } = default!;
    public Action StopLoading { get; set; } = default!;
}

public class PageState2( MainLayout layout )
{
    public MainLayout Layout { get; set; }
}