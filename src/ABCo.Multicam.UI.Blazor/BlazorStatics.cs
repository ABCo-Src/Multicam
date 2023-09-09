using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web.Services;
using ABCo.Multicam.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
