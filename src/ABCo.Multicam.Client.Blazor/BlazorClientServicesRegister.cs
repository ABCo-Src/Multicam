using ABCo.Multicam.Server.General;
using ABCo.Multicam.App.Win32.Services;
using ABCo.Multicam.Client.Services;

namespace ABCo.Multicam.Client.Blazor
{
	public static class BlazorClientServicesRegister
	{
		public static void AddServices(ClientServicesBuilder container)
		{
			container.AddScoped<IThreadDispatcher, BlazorMainThreadDispatcher>();
		}
	}
}
