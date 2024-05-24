using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Catalog;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<StorageService>();
//builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>( provider => provider.GetRequiredService<AuthenticationManager>() );
builder.Services.AddSingleton<CategoriesCache>();
builder.Services.AddScoped<CategoriesService>();
builder.ConfigureHttp();

await builder.Build().RunAsync();