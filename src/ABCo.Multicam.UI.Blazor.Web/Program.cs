using ABCo.Multicam.UI.Blazor;
using ABCo.Multicam.UI.Blazor.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

BlazorStatics.Initialize(builder.Services);

await builder.Build().RunAsync();