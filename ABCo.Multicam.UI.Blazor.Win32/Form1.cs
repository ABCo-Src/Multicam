using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web.Services;
using ABCo.Multicam.UI.Services;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using ABCo.Multicam.UI.Blazor.Win32.Services;

namespace ABCo.Multicam.UI.Blazor.Win32
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			MainThreadDispatcher.MainWindow = this;

			var services = new ServiceCollection();
			services.AddWindowsFormsBlazorWebView();
			services.AddBlazorWebViewDeveloperTools();
			services.AddSingleton<IServiceSource, ServiceSource>();
			services.AddSingleton<IUIWindow, UnwindowedUIWindow>();
			services.AddSingleton<IUIDialogHandler, UIDialogHandler>();
			services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();

			UIStatics.Initialize(services);

			blazorWebView1.HostPage = "wwwroot\\index.html";
			blazorWebView1.Services = services.BuildServiceProvider();
			blazorWebView1.RootComponents.Add<App>("#app");
		}
	}
}