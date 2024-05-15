using Microsoft.AspNetCore.Components;
using ShopApplication.Types;
using ShopWeb.Layout;

namespace ShopWeb.Pages;

internal abstract class PageBase : ComponentBase
{
    [Inject] MainLayout Layout { get; init; } = default!;

    protected void SetLoadingState( LoadingState state )
    {
        Layout.SetLoadingState( state );
    }
    protected void PushAlert( AlertType type, string message )
    {
        Layout.PushAlert( type, message );
    }
}