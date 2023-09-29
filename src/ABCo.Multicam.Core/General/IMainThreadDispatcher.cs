namespace ABCo.Multicam.Core.General
{
	public interface IMainThreadDispatcher 
	{
		void QueueOnMainFeatureThread(Action act);
		void QueueOnUIThread(Action act);
	}
}
