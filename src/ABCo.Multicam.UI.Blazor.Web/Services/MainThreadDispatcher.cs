using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.Blazor.Web;
using Microsoft.AspNetCore.Components;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class MainThreadDispatcher : IMainThreadDispatcher
	{
		public void QueueOnMainFeatureThread(Action act)
		{
			// Threading doesn't exist on web
			act();
		}
	}
}
