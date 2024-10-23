using ABCo.Multicam.Client.Blazor.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ABCo.Multicam.App.Win32.Services;
using ABCo.Multicam.Client.Blazor;
using ABCo.Multicam.Client;
using ABCo.Multicam.Server;
using ABCo.Multicam.UI.Blazor.Web.Services;

// Setup a server
var blazorDispatcher = new BlazorMainThreadDispatcher();
var server = new LocalMulticamServer(
    s => new WebPlatformInfo(),
    (c, s) => throw new Exception(),
    (a, s) => new LocalIPCollection(),
    blazorDispatcher);

// Setup our web client services
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var clientServiceBuilder = new ClientServicesBuilder(builder.Services, true);
builder.Services.AddSingleton(p => clientServiceBuilder.Build(p, blazorDispatcher, server, server.ServerInfo.ClientsManager.NewConnectionId()));
ClientServicesRegister.AddServices(clientServiceBuilder);
BlazorClientServicesRegister.AddServices(clientServiceBuilder);

// And build the app from this
builder.RootComponents.Add<ABCo.Multicam.Client.Blazor.Index>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
await builder.Build().RunAsync();