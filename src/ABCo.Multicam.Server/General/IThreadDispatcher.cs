namespace ABCo.Multicam.Server.General
{
    public interface IThreadDispatcher 
    {
        void Queue(Action act);
        Task Yield();
    }
}
