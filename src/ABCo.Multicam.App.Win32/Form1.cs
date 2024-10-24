using ABCo.Multicam.Server;
using ABCo.Multicam.App.Win32.Services;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using ABCo.Multicam.Client;
using ABCo.Multicam.Client.Blazor;
using ABCo.Multicam.Server.General.Factories;
using ABCo.Multicam.Client.Management;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.App.Win32
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Setup a server
            var blazorDispatcher = new FormDispatcher(this);
            var server = new ServerInfo(
                blazorDispatcher,
                (c, s) => new NativeServerHost(c, s),
                a => new AvailableIPCollection(a),
                new WindowsPlatformInfo());

            // Setup our desktop client services
            var desktopServiceCollection = new ServiceCollection();
            desktopServiceCollection.AddWindowsFormsBlazorWebView();
            desktopServiceCollection.AddBlazorWebViewDeveloperTools();
            desktopServiceCollection.AddSingleton<IClientInfo>(p => new ClientInfo(blazorDispatcher, new ServerConnection(server)));

            // And build the app from this
            var builtProvider = desktopServiceCollection.BuildServiceProvider();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = builtProvider;
            blazorWebView1.RootComponents.Add<Client.Blazor.Index>("#app");
        }

        // Workaround to fix the, currently broken, Blazor combobox down-drops when the window moves.
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            blazorWebView1.Padding = new Padding(1);
            blazorWebView1.PerformLayout();
            blazorWebView1.Padding = new Padding(0);
        }

        class FormDispatcher : IThreadDispatcher
        {
            readonly Form _form;
            public FormDispatcher(Form form) => _form = form;
            public void Queue(Action act) => _form.Invoke(act);
            public async Task Yield()
            {
                Application.DoEvents();
                await Task.Yield();
            }
        }
    }
}