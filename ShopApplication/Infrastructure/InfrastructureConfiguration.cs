using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShopApplication.Features.Identity;
using ShopApplication.Infrastructure.Http;
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