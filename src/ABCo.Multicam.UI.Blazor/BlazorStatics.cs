using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace ABCo.Multicam.UI.Blazor
{
	public static class BlazorStatics
	{
		public static void Initialize(IServiceCollection outerCollection)
		{
			IParameteredServiceCollection servSource = new TransientServiceRegister(outerCollection);
			servSource.AddSingleton<IServiceSource, ServiceSource>();
			servSource.AddSingleton<IUIWindow, UnwindowedUIWindow>();
			servSource.AddSingleton<IUIDialogHandler, UIDialogHandler>();
			UIStatics.Initialize(servSource);
		}

	}
}
