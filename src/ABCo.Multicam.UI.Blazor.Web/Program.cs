using ABCo.Multicam.Core;
using ABCo.Multicam.UI;
using ABCo.Multicam.UI.Blazor;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web;
using ABCo.Multicam.UI.Blazor.Web.Services;
using ABCo.Multicam.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IServiceSource, ServiceSource>();
builder.Services.AddSingleton<IUIWindow, UnwindowedUIWindow>();
builder.Services.AddSingleton<IUIDialogHandler, UIDialogHandler>();
UIStatics.Initialize(builder.Services);

await builder.Build().RunAsync();