using Microsoft.AspNetCore.Components;
using Shop.Infrastructure.Common;
using Shop.Infrastructure.Http;
using ShopApplication.Common;

namespace Shop.Shared;

public abstract class PageBase : ComponentBase
{
    protected PageBase() => redirectTime = GetRedirectTime( Configuration );

    [Inject] protected IConfiguration Configuration { get; init; } = default!;
    [Inject] protected IHttpService Http { get; init; } = default!;
    [Inject] protected NavigationManager Navigation { get; init; } = default!;
    [Inject] MainLayout layout { get; init; } = default!;

    readonly string ReturnUrl = string.Empty;
    readonly int redirectTime;
    int redirectCountdown = 0;
    System.Timers.Timer? pageRedirectTimer;
    
    protected string CurrentView = string.Empty;
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        StartLoading( "Loading Page..." );
    }

    protected void NavigateTo( NavigateToArgs args )
    {
        if (!args.ForceReload)
            CurrentView = args.Url;
        StateHasChanged();
        Navigation.NavigateTo( args.Url, args.ForceReload );
    }
    protected void StartRedirect( string? message )
    {
        layout.StartRedirecting( redirectTime, message );
        redirectCountdown = redirectTime;
        pageRedirectTimer = new System.Timers.Timer( redirectTime );
        pageRedirectTimer.Elapsed += CountdownTimerElapsed;
        pageRedirectTimer.AutoReset = true;
        pageRedirectTimer.Enabled = true;
    }
    protected void PushAlert( AlertType type, string message )
    {
        layout.PushAlert( type, message );
    }
    protected void PushAlert( PushAlertArgs args )
    {
        layout.PushAlert( args.Type, args.Message );
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
        if (redirectCountdown > 0) {
            redirectCountdown--;
            layout.TickRedirect();
            return;
        }

        pageRedirectTimer?.Stop();
        pageRedirectTimer?.Dispose();

        string returnUrl = !string.IsNullOrWhiteSpace( ReturnUrl ) ? ReturnUrl : string.Empty;
        Navigation.NavigateTo( returnUrl );
    }
    static int GetRedirectTime( IConfiguration configuration ) =>
        int.TryParse( configuration["PageRedirectTime"], out int time )
            ? time
            : 3;
}