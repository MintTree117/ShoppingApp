using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ShopApplication.Infrastructure.Http;

internal static class HttpConfiguration
{
    public static void ConfigureHttp( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped<IHttpService, HttpService>();
    }
}