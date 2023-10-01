using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.Blazor;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web;
using ABCo.Multicam.UI.Blazor.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<IPlatformInfo, WebPlatformInfo>();
builder.Services.AddSingleton<IThreadDispatcher, MainThreadDispatcher>();

//BlazorClientServicesRegister.BuildServiceRegister(builder.Services);

await builder.Build().RunAsync();