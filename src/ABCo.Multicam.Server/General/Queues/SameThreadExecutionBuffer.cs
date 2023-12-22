using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Queues
{
	public class SameThreadExecutionBuffer<T> : IExecutionBuffer<T>
	{
		readonly Queue<Delegate> _backlogQueue = new();
		readonly Action<Exception> _onError;
		readonly T _target;

		bool _currentlyRunning = false;

		public SameThreadExecutionBuffer(T target, Action<Exception> onError)
		{
			_target = target;
		}

		public void StartExecution() { }
		public void QueueFinish() { }
		public void QueueFinish(Action<T> finishAct) => QueueTask(finishAct);

		public void QueueTask(Action<T> act)
		{
			_backlogQueue.Enqueue(act);
			if (!_currentlyRunning) ProcessQueue();
		}

		public void QueueTaskAsync(Func<T, Task> act)
		{
			_backlogQueue.Enqueue(act);
			if (!_currentlyRunning) ProcessQueue();
		}

		public async void ProcessQueue()
		{
			_currentlyRunning = true;

			while (_backlogQueue.TryDequeue(out var backlogAct))
			{
				try
				{
					// If sync
					if (backlogAct is Action<T> backlogActSync)
						backlogActSync(_target);

					// If async
					else if (backlogAct is Func<T, Task> backlogActAsync)
						await backlogActAsync(_target);
				}
				catch (Exception ex) { _onError(ex); }
			}

			_currentlyRunning = false;
		}
	}
}
