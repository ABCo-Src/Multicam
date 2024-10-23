using ABCo.Multicam.Client;
using ABCo.Multicam.Client.Blazor;
using ABCo.Multicam.Client.Management;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Hosting.Management;

namespace ABCo.Multicam.App.Win32.Services
{
    public class NativeServerHost : INativeServerHost
    {
        readonly WebApplication _webApp;

        public NativeServerHost(NativeServerHostConfig config, IServerInfo info)
        {
            var webApp = WebApplication.CreateBuilder(new WebApplicationOptions()
            {
                ContentRootPath = AppContext.BaseDirectory
            });

            // Setup services
            webApp.Services.AddRazorPages();
            webApp.Services.AddServerSideBlazor();
            webApp.Services.AddScoped<IClientInfo>(p => new ClientInfo(new BlazorMainThreadDispatcher(), new ServerConnection(info)));

            // Build the app
            _webApp = webApp.Build();

            // Configure the HTTP request pipeline.
            if (!_webApp.Environment.IsDevelopment())
            {
                _webApp.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _webApp.UseHsts();
            }

            //app.UseHttpsRedirection();
            _webApp.UseStaticFiles();
            _webApp.UseRouting();
            _webApp.MapBlazorHub();
            _webApp.MapFallbackToPage("/_ClientWebIndex");

            // Set the host name
            _webApp.Urls.Clear();
            _webApp.Urls.Add(config.Host);
        }

        public async Task Start() => await _webApp.StartAsync();
        public async Task Stop() => await _webApp.StopAsync();
        public ValueTask DisposeAsync() => _webApp.DisposeAsync();
    }
}
