using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Client.Blazor.Services
{
    public class MainThreadDispatcher : IThreadDispatcher
    {
        public void Queue(Action act)
        {
            // Threading doesn't exist on web
            act();
        }
    }
}
