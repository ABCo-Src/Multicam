using ABCo.Multicam.Core;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Server;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Win32.Services;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.UI.Blazor.Win32
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			// Setup app-wide services (shared across everything, the desktop, the web clients etc.)
			var builder = AppWideServiceRegister.GetServiceBuilder();
			builder.AddSingleton<IPlatformInfo>(s => new WindowsPlatformInfo());
			builder.AddSingleton<IActiveServerHost>(s => new ActiveServerHost());
			BlazorStatics.Initialize(builder);

			// Now setup our local, desktop-specific service collection
			var provider = SetupDesktopServiceCollection();

			// Configure the control with this
			blazorWebView1.HostPage = "wwwroot\\index.html";
			blazorWebView1.Services = provider;
			blazorWebView1.RootComponents.Add<Index>("#app");

			var serverHost = provider.GetService<IServiceSource>()!.Get<IActiveServerHost>()!;
			serverHost.Connect("http://localhost:4000");
		}

		private static ServiceProvider SetupDesktopServiceCollection()
		{
			IServiceSource servSource = null!;
			var desktopServices = new ServiceCollection();

			// Add WinForms Blazor services
			desktopServices.AddWindowsFormsBlazorWebView();
			desktopServices.AddBlazorWebViewDeveloperTools();

			// Incorporate all our app-wide services in
			desktopServices.AddSingleton(p => servSource);
			AppWideServiceRegister.AddRegisteredScopedServicesTo(desktopServices, true);

			// Build provider and finish
			var builtDesktopServices = desktopServices.BuildServiceProvider();
			servSource = AppWideServiceRegister.GetWithScopesFrom(builtDesktopServices);
			return builtDesktopServices;
		}
	}
}