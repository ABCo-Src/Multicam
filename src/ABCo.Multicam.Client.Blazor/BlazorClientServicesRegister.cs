using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.Blazor.Services;
using ABCo.Multicam.App.Win32.Services;
using ABCo.Multicam.Client.Services;
using ABCo.Multicam.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Blazor
{
	public static class BlazorClientServicesRegister
	{
		public static void AddServices(ClientServicesBuilder container)
		{
			container.AddSingleton<IUIWindow>(new UnwindowedUIWindow());
			container.AddScoped<IUIDialogHandler, UIDialogHandler>();
			container.AddScoped<IThreadDispatcher, BlazorMainThreadDispatcher>();
		}
	}
}
