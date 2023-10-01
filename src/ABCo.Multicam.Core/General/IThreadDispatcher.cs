namespace ABCo.Multicam.Core.General
{
	public interface IThreadDispatcher 
	{
		void Queue(Action act);
	}
}
