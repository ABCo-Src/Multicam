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
	public static class BlazorStatics
	{
		public static void Initialize(IParameteredServiceCollection servSource)
		{
			servSource.AddSingleton<IUIWindow>(s => new UnwindowedUIWindow());
			servSource.AddSingleton<IUIDialogHandler>(s => new UIDialogHandler());
			servSource.AddScoped<IMainThreadDispatcher, BlazorMainThreadDispatcher>();
			UIStatics.Initialize(servSource);
		}
	}
}
