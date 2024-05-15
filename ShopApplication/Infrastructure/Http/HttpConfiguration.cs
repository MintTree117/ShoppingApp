using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ShopApplication.Infrastructure.Http;

internal static class HttpConfiguration
{
    internal static void ConfigureHttp( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );
        builder.Services.AddScoped<IHttpService, HttpService>();
    }
}