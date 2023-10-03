namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IServerTarget
    {
        IRemoteDataStore DataStore { get; }
        void PerformAction(int id);
        void PerformAction(int id, object param);
    }
}
