using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ABCo.Multicam.App.Win32.Services
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
