using ABCo.Multicam.Core.General;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class MainThreadDispatcher : IMainThreadDispatcher
	{
		public async void QueueOnMainFeatureThread(Action act)
		{
			// TODO: Handle exceptions
			var dispatcher = Dispatcher.CreateDefault();
			await dispatcher.InvokeAsync(act);
		}
	}
}
