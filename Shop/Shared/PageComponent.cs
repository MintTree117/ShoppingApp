using Microsoft.AspNetCore.Components;
using Shop.Infrastructure.Common;
using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;
using Shop.Utilities;

namespace Shop.Shared;

public abstract class PageComponent : ComponentBase
{
    [Inject] protected IHttpService Http { get; init; } = default!;
    [Parameter] public PageBase Page { get; set; } = default!;

    protected void GoHome() => 
        Redirect( Urls.PageHome );
    protected void StartLoading( string? loadingMessage = null )
    {
        Page.ToggleLoading( true, loadingMessage );
    }
    protected void StopLoading()
    {
        Page.ToggleLoading( false );
    }
    protected void PushSuccess( string message ) => PushAlert( AlertType.Success, message );
    protected void PushWarning( string message ) => PushAlert( AlertType.Warning, message );
    protected void PushError( string message ) => PushAlert( AlertType.Danger, message );
    protected void PushError( IOpt opt, string message ) => PushAlert( AlertType.Danger, $"{message} : {opt.Message()}" );
    protected void PushError( IOpt opt ) => PushAlert( AlertType.Danger, opt.Message() );
    protected void Navigate( string viewName, bool forceReload = false )
    {
        Page.Navigate( NavigationArgs.With( viewName, forceReload ) );
    }
    protected void Redirect( string url )
    {
        Page.Redirect( url );
    }

    void PushAlert( AlertType type, string message )
    {
        Page.PushAlert( PushAlertArgs.With( type, message ) );
    }
}