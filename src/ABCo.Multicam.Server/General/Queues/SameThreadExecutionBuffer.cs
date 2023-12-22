using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Queues
{
	public class SameThreadExecutionBuffer<T> : IExecutionBuffer<T>
	{
		readonly Queue<Action<T>> _backlogQueue = new();
		readonly T _target;

		bool _currentlyRunning = false;

		public SameThreadExecutionBuffer(T target) => _target = target;

		public void StartExecution() { }
		public void QueueFinish() { }
		public void QueueFinish(Action<T> finishAct) => QueueTask(finishAct);

		public void QueueTask(Action<T> act)
		{
			if (_currentlyRunning)
				_backlogQueue.Enqueue(act);
			else
			{
				_currentlyRunning = true;

				act(_target);
				while (_backlogQueue.TryDequeue(out var backlogAct)) backlogAct(_target);

				_currentlyRunning = false;
			}
		}
	}
}
