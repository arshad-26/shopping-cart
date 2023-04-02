using BlazorUI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Scoped Services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:38451/api/") });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.ClearAfterNavigation = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
});

await builder.Build().RunAsync();
