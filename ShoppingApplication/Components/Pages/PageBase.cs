using Microsoft.AspNetCore.Components;
using ShoppingApplication.Layout;
using ShoppingApplication.Types;

namespace ShoppingApplication.Components.Pages;

public abstract class PageBase : ComponentBase
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