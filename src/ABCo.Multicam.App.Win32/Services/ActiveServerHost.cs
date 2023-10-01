using ABCo.Multicam.Client;
using ABCo.Multicam.Client.Blazor;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.App.Win32.Services
{
    public class ActiveServerHost : IActiveServerHost
	{
		WebApplication _webApp;

		public ActiveServerHost(IServerInfo info)
		{
			var webApp = WebApplication.CreateBuilder(new WebApplicationOptions()
			{
				ContentRootPath = AppContext.BaseDirectory
			});

			// Setup services
			var builder = new ClientServicesBuilder(webApp.Services);
			webApp.Services.AddRazorPages();
			webApp.Services.AddServerSideBlazor();
			webApp.Services.AddScoped(p => builder.Build(p, new BlazorMainThreadDispatcher(), info.GetLocalClientConnection(), info.ClientsManager.NewConnectionId()));
			ClientServicesRegister.AddServices(builder);
			BlazorClientServicesRegister.AddServices(builder);

			// Build the app
			_webApp = webApp.Build();
		}

		public async void Connect(string hostPath)
		{
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
			await _webApp.RunAsync(hostPath);
		}
	}
}
