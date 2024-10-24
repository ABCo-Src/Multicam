﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Queues
{
    /// <summary>
    /// Represents an execution buffer, something that may have operations queued up to it, to then be executed specifically in the order given.
    /// This may be on a background thread, or it may just be the current thread but here to deal with async or cyclic requests.
    /// </summary>
    public interface IExecutionBuffer
    {
        void StartExecution();
        void QueueFinish();
        void QueueFinish(Action finishAct);
        void QueueTask(Action act);
        void QueueTaskAsync(Func<Task> act);
    }
}
