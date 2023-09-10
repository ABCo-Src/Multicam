using ABCo.Multicam.Core.General;
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

			MainThreadDispatcher.MainWindow = this;

			var services = new ServiceCollection();
			services.AddWindowsFormsBlazorWebView();
			services.AddBlazorWebViewDeveloperTools();
			services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();
			BlazorStatics.Initialize(services);

			blazorWebView1.HostPage = "wwwroot\\index.html";
			blazorWebView1.Services = services.BuildServiceProvider();
			blazorWebView1.RootComponents.Add<App>("#app");
		}
	}
}