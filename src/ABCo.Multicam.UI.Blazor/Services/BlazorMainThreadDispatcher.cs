using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ABCo.Multicam.UI.Blazor.Win32.Services
{
	public class BlazorMainThreadDispatcher : IThreadDispatcher
	{
		Func<Action, Task> _invoker = null!;

		public BlazorMainThreadDispatcher()
		{
			Console.WriteLine("Hmmm");
		}

		public void Associate(Func<Action, Task> invoker) => _invoker = invoker;

		public async void Queue(Action act) => await _invoker(act);
		public async void QueueOnUIThread(Action act) => await _invoker(act);
	}
}
