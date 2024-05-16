using Microsoft.AspNetCore.Components;
using ShopApplication.Common;
using ShopApplication.Infrastructure.Http;
using ShopWeb.Layout;

namespace ShopWeb.Pages;

public abstract class PageBase : ComponentBase
{
    protected PageBase() => redirectTime = GetRedirectTime( Configuration );
    
    protected IConfiguration Configuration { get; init; } = default!;
    protected IHttpService Http { get; init; } = default!;
    [Inject] MainLayout layout { get; init; } = default!;
    [Inject] NavigationManager navigation { get; init; } = default!;

    readonly string ReturnUrl = string.Empty;
    readonly int redirectTime;
    int countdown = 0;
    System.Timers.Timer? pageRedirectTimer;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Start();
    }
    protected virtual void Start()
    {
        StopLoading();
    }
    
    protected void StartRedirect( string? message )
    {
        layout.StartRedirecting( redirectTime, message );
        countdown = redirectTime;
        pageRedirectTimer = new System.Timers.Timer( redirectTime );
        pageRedirectTimer.Elapsed += CountdownTimerElapsed;
        pageRedirectTimer.AutoReset = true;
        pageRedirectTimer.Enabled = true;
    }
    protected void PushAlert( AlertType type, string message )
    {
        layout.PushAlert( type, message );
    }
    protected void StartLoading( string? message )
    {
        layout.StartLoading( message );
    }
    protected void StopLoading()
    {
        layout.HideLoader();
    }
    
    void CountdownTimerElapsed( object? sender, System.Timers.ElapsedEventArgs e )
    {
        if (countdown > 0) {
            countdown--;
            layout.TickRedirect();
            return;
        }

        pageRedirectTimer?.Stop();
        pageRedirectTimer?.Dispose();

        string returnUrl = !string.IsNullOrWhiteSpace( ReturnUrl ) ? ReturnUrl : string.Empty;
        navigation.NavigateTo( returnUrl );
    }
    static int GetRedirectTime( IConfiguration configuration ) =>
        int.TryParse( configuration["PageRedirectTime"], out int time )
            ? time
            : 3;
}