using BlazorUI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Scoped Services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:38451/api/") });

builder.Services.AddMudServices();

await builder.Build().RunAsync();
