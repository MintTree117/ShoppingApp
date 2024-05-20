using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Shop.Infrastructure.Http;

internal static class HttpConfiguration
{
    public static void ConfigureHttp( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( GetBaseUrl( builder ) ) } );
        builder.Services.AddScoped<HttpService>();
    }

    static string GetBaseUrl( WebAssemblyHostBuilder builder ) =>
        builder.Configuration["BaseUrl"] ?? throw new Exception( "Failed to get base url from IConfiguration during startup." );
}