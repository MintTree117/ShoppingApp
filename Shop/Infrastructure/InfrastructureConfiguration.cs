using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Identity;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure;

internal static class InfrastructureConfiguration
{
    internal static void ConfigureInfrastructure( this WebAssemblyHostBuilder builder )
    {
        builder.ConfigureStorage();
        builder.ConfigureHttp();
        builder.ConfigureIdentity();
    }
}