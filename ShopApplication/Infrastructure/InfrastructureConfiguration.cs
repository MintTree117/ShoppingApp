using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShopApplication.Infrastructure.Http;
using ShopApplication.Infrastructure.Identity;
using ShopApplication.Infrastructure.Storage;

namespace ShopApplication.Infrastructure;

public static class InfrastructureConfiguration
{
    public static void ConfigureInfrastructure( this WebAssemblyHostBuilder builder )
    {
        builder.ConfigureHttp();
        builder.ConfigureIdentity();
        builder.ConfigureStorage();
    }
}