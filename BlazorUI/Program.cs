using BlazorUI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorUI.Providers;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using BlazorUI.Interceptors;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Scoped Services
builder.Services.AddScoped<AuthenticationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthenticationProvider>());
builder.Services.AddScoped<AuthInterceptor>();

builder.Services.AddHttpClient("ServerAPI", (serviceProvider, client) => {
    client.BaseAddress = new Uri("http://localhost:38451/api/");
    client.EnableIntercept(serviceProvider);
});

builder.Services.AddHttpClient("RefreshAPI", client => client.BaseAddress = new Uri("http://localhost:38451/api/"));

builder.Services.AddHttpClientInterceptor();
builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.ClearAfterNavigation = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
});

builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();
