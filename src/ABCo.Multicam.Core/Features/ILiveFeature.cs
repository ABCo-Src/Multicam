namespace ABCo.Multicam.Core.Features
{
	public interface ILiveFeature : IDisposable
    {
        void PerformAction(int id);
        void PerformAction(int id, object param);
    }
}
