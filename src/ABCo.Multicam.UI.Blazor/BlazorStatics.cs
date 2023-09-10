using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web.Services;
using ABCo.Multicam.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.UI.Blazor
{
	public static class BlazorStatics
	{
		public static void Initialize(IServiceCollection outerCollection)
		{
			var serviceCollection = new TransientServiceRegister(outerCollection);

			serviceCollection.AddSingleton<IServiceSource, ServiceSource>();
			serviceCollection.AddSingleton<IUIWindow, UnwindowedUIWindow>();
			serviceCollection.AddSingleton<IUIDialogHandler, UIDialogHandler>();
			UIStatics.Initialize(serviceCollection);
		}

	}
}
