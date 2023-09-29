using ABCo.Multicam.Core;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Server;
using ABCo.Multicam.UI.Blazor.Services;

namespace ABCo.Multicam.UI.Blazor.Win32.Services
{
	public class ActiveServerHost : IActiveServerHost
	{
		WebApplication _webApp;

		public ActiveServerHost()
		{
			var webApp = WebApplication.CreateBuilder(new WebApplicationOptions()
			{
				ContentRootPath = AppContext.BaseDirectory
			});

			// Blazor Service-specific services
			webApp.Services.AddRazorPages();
			webApp.Services.AddServerSideBlazor();

			// Add app-wide services to collection
			IServiceSource servSource = null!;
			webApp.Services.AddSingleton(p => servSource);
			AppWideServiceRegister.AddRegisteredScopedServicesTo(webApp.Services, false);

			// Build
			_webApp = webApp.Build();
			servSource = AppWideServiceRegister.GetWithScopesFrom(_webApp.Services);
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
