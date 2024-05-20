using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Identity;
using Shop.Infrastructure.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<AuthenticationManager>();
builder.Services.AddScoped<AuthenticationStateProvider>( provider => provider.GetRequiredService<AuthenticationManager>() );
builder.ConfigureHttp();

await builder.Build().RunAsync();