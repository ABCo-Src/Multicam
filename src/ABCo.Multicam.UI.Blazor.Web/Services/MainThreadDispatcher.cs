using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.Blazor.Web;
using Microsoft.AspNetCore.Components;

namespace ABCo.Multicam.Client.Blazor.Services
{
	public class MainThreadDispatcher : IThreadDispatcher
	{
		public void Queue(Action act)
		{
			// Threading doesn't exist on web
			act();
		}
	}
}
