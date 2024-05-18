using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ShopApplication.Features.Identity;

internal static class IdentityConfiguration
{
    internal static void ConfigureIdentity( this WebAssemblyHostBuilder builder )
    {
        builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationManager>();
    }
}