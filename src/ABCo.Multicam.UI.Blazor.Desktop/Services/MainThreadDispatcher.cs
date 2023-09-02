using ABCo.Multicam.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
