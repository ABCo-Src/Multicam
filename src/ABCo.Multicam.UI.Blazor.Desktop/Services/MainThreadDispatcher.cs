using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class MainThreadDispatcher : IMainThreadDispatcher
	{
		public void QueueOnMainFeatureThread(Action act)
		{
			MainThread.BeginInvokeOnMainThread(act);
		}
	}
}
