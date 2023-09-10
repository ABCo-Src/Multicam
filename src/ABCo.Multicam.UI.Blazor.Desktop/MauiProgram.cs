using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.Blazor.Services;
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

			builder.Services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();

			BlazorStatics.Initialize(builder.Services);

			return builder.Build();
		}
	}
}