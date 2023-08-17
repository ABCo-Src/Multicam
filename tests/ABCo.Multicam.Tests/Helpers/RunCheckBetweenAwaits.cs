namespace ABCo.Multicam.Tests.Helpers
{
    public class RunCheckBetweenAwaits : SynchronizationContext
    {
        Exception? _thrownException;

        readonly Action _between;
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

        public static RunCheckBetweenAwaits SetupCheck(Action action)
        {
            var context = new RunCheckBetweenAwaits(action);
            SetSynchronizationContext(context);
            return context;
        }
    }
}
