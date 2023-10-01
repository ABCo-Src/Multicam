using ABCo.Multicam.Core;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Win32.Services;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace ABCo.Multicam.UI.Blazor
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
