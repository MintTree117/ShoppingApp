using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Shop.Infrastructure.Storage;

internal static class StorageConfiguration
{
    internal static void ConfigureStorage( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped<StorageService>();
    }
}