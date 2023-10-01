using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class MainThreadDispatcher : IThreadDispatcher
	{
		public void Queue(Action act)
		{
			MainThread.BeginInvokeOnMainThread(act);
		}
	}
}
