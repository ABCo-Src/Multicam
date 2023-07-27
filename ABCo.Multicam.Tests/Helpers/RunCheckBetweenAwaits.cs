using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Helpers
{
    public class RunCheckBetweenAwaits : SynchronizationContext
    {
        Exception? _thrownException;

        Action _between;
        public RunCheckBetweenAwaits(Action between) => _between = between;

        public void AssertNoFail()
        {
            if (_thrownException != null)
                throw new Exception("Exception thrown (in between-await checks): " + _thrownException.Message);            
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            if (_thrownException == null)
            {
                try
                {
                    _between();
                }
                catch (Exception ex)
                {
                    _thrownException = ex;
                }
            }
            
            base.Post(d, state);
        }

        /// <summary>
        /// Sets up a check. The given function will be called around every await, and if it returns true at any point, the action is considered to have failed.
        /// </summary>        
        public static RunCheckBetweenAwaits SetupCheck(Action actionRetIfOk)
        {
            var context = new RunCheckBetweenAwaits(actionRetIfOk);
            SetSynchronizationContext(context);
            return context;
        }
    }
}
