using ABCo.Multicam.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
