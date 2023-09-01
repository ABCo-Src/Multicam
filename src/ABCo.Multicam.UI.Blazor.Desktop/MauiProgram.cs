using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Blazor.Services;
using ABCo.Multicam.UI.Blazor.Web.Services;
using ABCo.Multicam.UI.Services;
using Microsoft.Extensions.Logging;

namespace ABCo.Multicam.UI.Blazor.Desktop
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			builder.Services.AddSingleton<IServiceSource, ServiceSource>();
			builder.Services.AddSingleton<IUIWindow, UnwindowedUIWindow>();
			builder.Services.AddSingleton<IUIDialogHandler, UIDialogHandler>();

			UIStatics.Initialize(builder.Services);

			return builder.Build();
		}
	}
}