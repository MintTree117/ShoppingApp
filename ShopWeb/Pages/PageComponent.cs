using Microsoft.AspNetCore.Components;
using ShopApplication.Common;
using ShopApplication.Infrastructure.Http;

namespace ShopWeb.Pages;

public abstract class PageComponent : ComponentBase
{
    [Inject] protected IHttpService Http { get; init; } = default!;
    
    [Parameter] public EventCallback<string> StartLoadingCallback { get; init; } = default!;
    [Parameter] public EventCallback StopLoadingCallback { get; init; } = default!;
    [Parameter] public EventCallback<PushAlertArgs> PushAlertCallback { get; init; } = default!;
    [Parameter] public EventCallback<NavigateToArgs> NavigateToCallback { get; init; } = default!;

    protected async Task CallStartLoading( string loadingMessage )
    {
        await StartLoadingCallback.InvokeAsync( loadingMessage );
    }
    protected async Task CallStopLoading()
    {
        await StopLoadingCallback.InvokeAsync();
    }
    protected async Task CallPushAlert( AlertType type, string message )
    {
        await PushAlertCallback.InvokeAsync( PushAlertArgs.With( type, message ) );
    }
    protected async Task CallNavigateTo( string url, bool forceReload )
    {
        await NavigateToCallback.InvokeAsync( NavigateToArgs.With( url, forceReload ) );
    }

    public readonly record struct PushAlertArgs(
        AlertType Type,
        string Message )
    {
        internal static PushAlertArgs With( AlertType type, string message ) =>
            new( type, message );
    }
    public readonly record struct NavigateToArgs(
        string Url,
        bool ForceReload )
    {
        internal static NavigateToArgs With( string url, bool isReal ) =>
            new( url, isReal );
    }
}