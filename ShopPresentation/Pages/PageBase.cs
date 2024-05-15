using Microsoft.AspNetCore.Components;
using ShopApplication.Types;
using ShopPresentation.Layout;

namespace ShopPresentation.Pages;

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