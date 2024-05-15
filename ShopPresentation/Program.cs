using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShopApplication.Infrastructure;
using ShopPresentation;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.ConfigureInfrastructure();
builder.Services.AddAuthorizationCore();
await builder.Build().RunAsync();