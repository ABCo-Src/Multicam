using System.Diagnostics;

namespace ABCo.Multicam.Server.General.Queues
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

    public class BackgroundThreadExecutionBuffer : IExecutionBuffer
    {
        readonly Queue<Action?> _taskQueue; // Used as the lock for everything here
        readonly Action<Exception> _errorHandler;

        volatile Action? _finishAction;
        volatile bool _isRunning = false;

        public BackgroundThreadExecutionBuffer(Action<Exception> errorHandler)
        {
            _errorHandler = errorHandler;
			_taskQueue = new();
        }

        public void StartExecution()
        {
            lock (_taskQueue)
            {
                if (_isRunning) throw new Exception("Attempted to start running background thread when it's already running!");
                _isRunning = true;

                var thread = new Thread(() => DoWorkBackground());

                // Only do the STA part if we're on Windows
                if (OperatingSystem.IsWindows())
                    thread.SetApartmentState(ApartmentState.STA);

                thread.Start();
            }
        }

        void DoWorkBackground()
        {
            while (true)
            {
                // Wait for a task to come in the queue, and stop if it's a stop request
                var item = WaitForItem();
                if (item == null) break;

                // Process the item
                try
                {
                    item();
                }
                catch (Exception ex) { _errorHandler(ex); }
            }
        }

        private Action? WaitForItem()
        {
            var waiter = new SpinWait();

            while (true)
            {
                lock (_taskQueue)
                {
                    if (_taskQueue.TryDequeue(out var item))
                        return item;
                }

                waiter.SpinOnce();
            }
        }

        public void QueueFinish()
        {
            lock (_taskQueue)
                _taskQueue.Enqueue(null);
        }

        public void QueueFinish(Action finishAct)
        {
            lock (_taskQueue)
            {
                if (!_isRunning) goto RunOnThisThread;

                _taskQueue.Enqueue(finishAct);
                _taskQueue.Enqueue(null);
                return;
            }

        RunOnThisThread:
            finishAct();
        }

        public void QueueTask(Action act)
        {
            lock (_taskQueue)
                _taskQueue.Enqueue(act);
        }

		public void QueueTaskAsync(Func<Task> act)
		{
			throw new NotImplementedException();
		}
	}
}
