using ABCo.Multicam.Core;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Server;
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

			// Setup a server
			var blazorDispatcher = new BlazorMainThreadDispatcher();
			var server = new LocalMulticamServer(s => new WindowsPlatformInfo(), s => new ActiveServerHost(s), blazorDispatcher);

			// Setup our desktop client services
			var desktopServiceCollection = new ServiceCollection();
			var clientServiceBuilder = new ClientServicesBuilder(desktopServiceCollection, true);
			desktopServiceCollection.AddWindowsFormsBlazorWebView();
			desktopServiceCollection.AddBlazorWebViewDeveloperTools();
			desktopServiceCollection.AddSingleton(p => clientServiceBuilder.Build(p, blazorDispatcher, server, server.ServerInfo.ClientsManager.NewConnectionId()));
			ClientServicesRegister.AddServices(clientServiceBuilder);
			BlazorClientServicesRegister.AddServices(clientServiceBuilder);

			// And build the app from this
			var builtProvider = desktopServiceCollection.BuildServiceProvider();
			blazorWebView1.HostPage = "wwwroot\\index.html";
			blazorWebView1.Services = builtProvider;
			blazorWebView1.RootComponents.Add<Index>("#app");

			var serverHost = server.ServerInfo.Get<IActiveServerHost>();
			serverHost.Connect("http://10.149.237.129:4000");
		}

		//class FormDispatcher : IThreadDispatcher
		//{
		//	readonly Form _form;
		//	public FormDispatcher(Form form) => _form = form;
		//	public void Queue(Action act) => _form.Invoke(act);
		//}
	}
}