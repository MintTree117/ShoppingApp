using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShoppingApplication;
using ShoppingApplication.Infrastructure.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityManager>();
await builder.Build().RunAsync();