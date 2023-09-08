using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.UI.Blazor.Win32.Services
{
	public class MainThreadDispatcher : IMainThreadDispatcher
	{
		public static Control MainWindow = null!;
		public void QueueOnMainFeatureThread(Action act)
		{
			MainWindow.BeginInvoke(act);
		}
	}
}
