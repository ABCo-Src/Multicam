using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Queues
{
    public class SameThreadExecutionBuffer : IExecutionBuffer
    {
        readonly Queue<Delegate> _backlogQueue = new();
        readonly Action<Exception> _onError;

        bool _currentlyRunning = false;

        public SameThreadExecutionBuffer(Action<Exception> onError) => _onError = onError;

        public void StartExecution() { }
        public void QueueFinish() { }
        public void QueueFinish(Action finishAct) => QueueTask(finishAct);

        public void QueueTask(Action act)
        {
            _backlogQueue.Enqueue(act);
            if (!_currentlyRunning) ProcessQueue();
        }

        public void QueueTaskAsync(Func<Task> act)
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
                    if (backlogAct is Action backlogActSync)
                        backlogActSync();

                    // If async
                    else if (backlogAct is Func<Task> backlogActAsync)
                        await backlogActAsync();
                }
                catch (Exception ex) { _onError(ex); }
            }

            _currentlyRunning = false;
        }
    }
}
