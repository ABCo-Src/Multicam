using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.Blazor;
using ABCo.Multicam.Client.Blazor.Services;
using ABCo.Multicam.Client.Blazor.Web;
using ABCo.Multicam.Client.Blazor.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<IPlatformInfo, WebPlatformInfo>();
builder.Services.AddSingleton<IThreadDispatcher, MainThreadDispatcher>();

//BlazorClientServicesRegister.BuildServiceRegister(builder.Services);

await builder.Build().RunAsync();