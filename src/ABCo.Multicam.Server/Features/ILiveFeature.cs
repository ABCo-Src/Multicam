namespace ABCo.Multicam.Server.Features
{
	public interface ILiveFeature : IDisposable
	{
		void PerformAction(int id);
		void PerformAction(int id, object param);
	}
}
