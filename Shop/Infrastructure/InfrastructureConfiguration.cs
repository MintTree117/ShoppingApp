using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop.Features.Identity;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure;

public static class InfrastructureConfiguration
{
    public static void ConfigureInfrastructure( this WebAssemblyHostBuilder builder )
    {
        builder.ConfigureHttp();
        builder.ConfigureIdentity();
        builder.ConfigureStorage();
    }
}