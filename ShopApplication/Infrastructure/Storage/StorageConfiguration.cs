using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ShopApplication.Infrastructure.Storage;

internal static class StorageConfiguration
{
    internal static void ConfigureStorage( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped<IStorageService, StorageService>();
    }
}