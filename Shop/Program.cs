using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Catalog;
using Shop.Infrastructure.Catalog.Categories;
using Shop.Infrastructure.Common;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Configuration.AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );

builder.ConfigureHttp();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<StorageService>();
//builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<AuthProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>( provider => provider.GetRequiredService<AuthenticationManager>() );
builder.Services.AddScoped<LoadingService>();
builder.Services.AddSingleton<CategoriesCache>();
builder.Services.AddScoped<CategoriesService>();

await builder.Build().RunAsync();