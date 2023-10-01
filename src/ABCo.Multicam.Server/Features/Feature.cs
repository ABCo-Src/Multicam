using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Interaction;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.Client.ViewModels.Features;

namespace ABCo.Multicam.Server.Features
{
    /// <summary>
    /// Represents a feature currently loaded, whether running locally on this system or remotely.
    /// Introduces the entire data fragments system, as well as fragments like the "Title" or "Item" within there
    /// </summary>
    public interface IFeature : IServerService<FeatureTypes, IFeatureDataSource, IFeatureActionTarget>, IServerTarget, IDisposable
	{
	}

	public class Feature : IFeature, IFeatureDataChangeEventHandler
	{
		readonly IFeatureDataSource _dataSource; // The raw data source. May be local or may be remote.
		readonly IFeatureActionTarget _actionTarget; // The target to direct actions towards. May go to a locally-running feature, or may be remote.
		readonly IClientNotifier _clientTargets;

		public IRemoteClientNotifier ClientMessageDispatcher => _clientTargets;

		public Feature(FeatureTypes featureType, IFeatureDataSource dataSource, IFeatureActionTarget actionTarget, IServerInfo servSource)
		{
			_dataSource = dataSource;
			_dataSource.SetDataChangeHandler(this);
			_actionTarget = actionTarget;

            // Create the UI presenter
            _clientTargets = servSource.ClientsManager.NewClientsDataNotifier(this);
		}

		public void RefreshData<T>() where T : ServerData => _dataSource.RefreshData<T>();
		public void PerformAction(int id) => _actionTarget.PerformAction(id);
		public void PerformAction(int id, object param) => _actionTarget.PerformAction(id, param);
		public void OnDataChange(ServerData val) => _clientTargets.OnDataChange(val);

		public void Dispose()
		{
			_clientTargets.Dispose();
			_actionTarget.Dispose();
		}
	}
}