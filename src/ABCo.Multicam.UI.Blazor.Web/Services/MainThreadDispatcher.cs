using ABCo.Multicam.Server.General;
using ABCo.Multicam.UI.Blazor.Web;
using Microsoft.AspNetCore.Components;

namespace ABCo.Multicam.UI.Blazor.Services
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
