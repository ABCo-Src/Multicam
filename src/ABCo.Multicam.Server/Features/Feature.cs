using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features
{
    /// <summary>
    /// Represents a feature currently loaded, whether running locally on this system or remotely.
    /// Introduces the entire data fragments system, as well as fragments like the "Title" or "Item" within there
    /// </summary>
    public interface IFeature : IServerService<FeatureTypes>, IServerTarget, IDisposable
	{
	}

	public interface IFeatureDataStore
	{
		T GetData<T>() where T : ServerData;
		void SetData<T>(ServerData val) where T : ServerData;
	}

	public class Feature : IFeature, IFeatureDataStore
	{
		readonly ILiveFeature _runningFeature; // The "running logic" of this feature
		readonly IClientSyncedDataStore _dataSource; // The (client-shared) data associated with this feature

		public IRemoteDataStore DataStore => _dataSource;

		public Feature(FeatureTypes featureType, IServerInfo servSource)
		{
            _dataSource = servSource.ClientsManager.NewClientsDataNotifier(this);
			_runningFeature = servSource.Get<IFeatureContentFactory>().GetLiveFeature(featureType, this);
		}

		public void PerformAction(int id) => _runningFeature.PerformAction(id);
		public void PerformAction(int id, object param) => _runningFeature.PerformAction(id, param);
		public T GetData<T>() where T : ServerData => _dataSource.GetData<T>();
		public void SetData<T>(ServerData val) where T : ServerData => _dataSource.SetData<T>(val);

		public void Dispose()
		{
			_dataSource.Dispose();
			_runningFeature.Dispose();
		}
	}
}