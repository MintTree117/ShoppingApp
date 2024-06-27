using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Shop.Infrastructure.Http;

public sealed class CookieDelegatingHandler( ILogger<CookieDelegatingHandler> logger ) : DelegatingHandler
{
    readonly ILogger<CookieDelegatingHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        request.SetBrowserRequestCredentials( BrowserRequestCredentials.Include );
        return await base.SendAsync( request, cancellationToken );
    }
}