using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop.Utilities;

namespace Shop.Infrastructure.Http;

internal static class HttpConfiguration
{
    public static void ConfigureHttp( this WebAssemblyHostBuilder builder )
    {
        builder.Services
            .AddTransient<CookieDelegatingHandler>()
            .AddScoped( sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient( "API" ) )
            .AddHttpClient( "API", client => client.BaseAddress = new Uri( Consts.ApiBase ) ).AddHttpMessageHandler<CookieDelegatingHandler>();
        
        builder.Services.AddSingleton<HttpService>();

    }

    static string GetBaseUrl( WebAssemblyHostBuilder builder ) =>
        builder.Configuration["BaseUrl"] ?? throw new Exception( "Failed to get base url from IConfiguration during startup." );
}