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

    public class BackgroundThreadExecutionBuffer<T> : IExecutionBuffer<T>
    {
        readonly Queue<Action<T>?> _taskQueue; // Used as the lock for everything here
        readonly bool _tryRunUnderSTA;
        readonly T _target;

        volatile Action<T>? _finishAction;
        volatile bool _isRunning = false;

        public BackgroundThreadExecutionBuffer(bool tryRunUnderSTA, T target)
        {
            _target = target;
            _tryRunUnderSTA = tryRunUnderSTA;
            _taskQueue = new();
        }

        public void StartExecution()
        {
            lock (_taskQueue)
            {
                if (_isRunning) throw new Exception("Attempted to start running background thread when it's already running!");
                _isRunning = true;

                var thread = new Thread(() => DoWorkBackground(_target));

                // Only do the STA part if we're on Windows
                if (_tryRunUnderSTA && OperatingSystem.IsWindows())
                    thread.SetApartmentState(ApartmentState.STA);

                thread.Start();
            }
        }

        void DoWorkBackground(T target)
        {
            while (true)
            {
                // Wait for a task to come in the queue, and stop if it's a stop request
                var item = WaitForItem(target);
                if (item == null) break;

                // Process the item
                item(target);
            }
        }

        private Action<T>? WaitForItem(T target)
        {
            Action<T>? stopAction;
            var waiter = new SpinWait();

            while (true)
            {
                lock (_taskQueue)
                {
                    if (_taskQueue.TryDequeue(out var item))
                    {
                        if (item == null)
                        {
                            stopAction = _finishAction;
                            _finishAction = null;
                            _isRunning = false;
                            return null; // Jump to the end
                        }

                        return item;
                    }
                }

                waiter.SpinOnce();
            }
        }

        public void QueueFinish()
        {
            lock (_taskQueue)
                _taskQueue.Enqueue(null);
        }

        public void QueueFinish(Action<T> finishAct)
        {
            lock (_taskQueue)
            {
                if (!_isRunning) goto RunOnThisThread;

                _taskQueue.Enqueue(finishAct);
                _taskQueue.Enqueue(null);
                return;
            }

        RunOnThisThread:
            finishAct(_target);
        }

        public void QueueTask(Action<T> act)
        {
            lock (_taskQueue)
                _taskQueue.Enqueue(act);
        }
    }
}
