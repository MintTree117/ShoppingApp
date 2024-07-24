using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop;
using Shop.Infrastructure;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Configuration.AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );

builder.ConfigureHttp();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddSingleton<StorageService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateManager>();
builder.Services.AddSingleton<CartManager>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>( provider => provider.GetRequiredService<AuthenticationManager>() );
builder.Services.AddScoped<LoadingService>();
builder.Services.AddSingleton<LocationManager>();
builder.Services.AddSingleton<CategoriesCache>();
builder.Services.AddSingleton<BrandsCache>();

await builder.Build().RunAsync();