namespace ABCo.Multicam.Core.General
{
	public interface ICatchingOrderedBackgroundQueue<T> where T : IErrorHandlingTarget
	{
		void StartExecution();
		void QueueTask(Action<T> act, T target);
	}

	public interface IErrorHandlingTarget
	{
		void ProcessError(Exception ex);
	}

	public class CatchingAndQueuedSTAThread<T> : ICatchingOrderedBackgroundQueue<T> where T : IErrorHandlingTarget
	{
		record struct QueueItem(Action<T>? ToPerform, T Target);

		Queue<QueueItem> _taskQueue;

		public CatchingAndQueuedSTAThread()
		{
			_taskQueue = new();
		}

		public void StartExecution()
		{
			var thread = new Thread(DoWorkBackground);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		public void DoWorkBackground()
		{
			while (true)
			{
				// Wait for a task to come in the queue, and stop if it's a stop request
				var item = WaitForItem();
				if (item.ToPerform == null) break;

				// Process the item
				try
				{
					item.ToPerform(item.Target);
				}
				catch (Exception ex)
				{
					item.Target.ProcessError(ex);
				}
			}
		}

		private QueueItem WaitForItem()
		{
			var waiter = new SpinWait();
			while (true)
			{
				lock (_taskQueue)
				{
					// TODO: Investigate, would it be better to move the return outside the lock?
					if (_taskQueue.TryDequeue(out var item))
						return item;
				}

				waiter.SpinOnce();
			}
		}

		public void QueueFinish()
		{
			lock (_taskQueue)
				_taskQueue.Enqueue(new(null, default!));
		}

		public void QueueTask(Action<T> act, T target)
		{
			lock (_taskQueue) 
				_taskQueue.Enqueue(new(act, target));
		}
	}
}
