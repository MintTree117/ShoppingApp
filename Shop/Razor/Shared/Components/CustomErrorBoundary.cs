using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Shop.Infrastructure.Loading;

namespace Shop.Razor.Shared.Components;

public sealed class CustomErrorBoundary : ErrorBoundary
{
    [Parameter]
    public NotificationsPanel Panel { get; init; } = default!;
    
    protected override async Task OnErrorAsync( Exception exception )
    {
        //await base.OnErrorAsync( exception );
        Panel.PushAlert( AlertType.Danger, $"An unhandled error occured. {exception}" );
    }
}