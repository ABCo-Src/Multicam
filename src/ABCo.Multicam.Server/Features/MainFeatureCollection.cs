using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Interaction;
using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.Client.ViewModels.Features;

namespace ABCo.Multicam.Server.Features
{
    /// <summary>
    /// Manages all the (running) features in the current project.
    /// </summary>
    public interface IMainFeatureCollection : IServerTarget, IDisposable
    {
        IReadOnlyList<IFeature> Features { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

    public class MainFeatureCollection : IMainFeatureCollection
    {
        public const int CREATE = 0;
        public const int MOVE_UP = 1;
        public const int MOVE_DOWN = 2;
        public const int DELETE = 3;

        public static MainFeatureCollection? AppWideInstance { get; set; }

        readonly IServerInfo _servSource;
        readonly IFeatureContentFactory _featureContentFactory;
        readonly IClientNotifier _clientTargets;
		readonly List<IFeature> _features = new();

        public IReadOnlyList<IFeature> Features => _features;
		public IRemoteClientNotifier ClientMessageDispatcher => _clientTargets;

		public MainFeatureCollection(IServerInfo source)
        {
            _servSource = source;
            _featureContentFactory = source.Get<IFeatureContentFactory>();
			_clientTargets = source.ClientsManager.NewClientsDataNotifier(this);
        }

        public void CreateFeature(FeatureTypes type)
        {
            // Create the data store 
            var dataSpecs = _featureContentFactory.GetFeatureDataEntries(type);
			var dataStore = _servSource.Get<ILocallyInitializedFeatureDataSource, FeatureDataInfo[]>(dataSpecs);

            // Create the live feature
            var liveFeature = _featureContentFactory.GetLiveFeature(type, dataStore);

            // Add the feature
			_features.Add(_servSource.Get<IFeature, FeatureTypes, IFeatureDataSource, IFeatureActionTarget>(type, dataStore, liveFeature));
			RefreshFeaturesList();
		}

        public void MoveUp(IFeature feature)
        {
            int indexOfFeature = _features.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_features[indexOfFeature], _features[indexOfFeature - 1]) = (_features[indexOfFeature - 1], _features[indexOfFeature]);

            RefreshFeaturesList();
        }

        public void MoveDown(IFeature feature)
        {
            int indexOfFeature = _features.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _features.Count - 1) return;

            (_features[indexOfFeature], _features[indexOfFeature + 1]) = (_features[indexOfFeature + 1], _features[indexOfFeature]);

			RefreshFeaturesList();
		}

        public void Delete(IFeature feature)
        {
            _features.Remove(feature);
            feature.Dispose();

			RefreshFeaturesList();
		}

        public void Dispose()
        {
			_clientTargets.Dispose();

            for (int i = 0; i < _features.Count; i++)
                _features[i].Dispose();
        }

        public void PerformAction(int id) => throw new NotSupportedException();
		public void PerformAction(int id, object param)
		{
            switch (id)
            {
                case CREATE:
                    CreateFeature((FeatureTypes)param);
                    break;
				case MOVE_UP:
					MoveUp((IFeature)param);
					break;
				case MOVE_DOWN:
					MoveDown((IFeature)param);
					break;
				case DELETE:
					Delete((IFeature)param);
					break;
                default:
					throw new NotSupportedException();
			}
		}

		public void RefreshData<T>() where T : ServerData
		{
            if (typeof(T) == typeof(FeaturesList)) RefreshFeaturesList();
		}

        void RefreshFeaturesList() => _clientTargets.OnDataChange(new FeaturesList(_features.Cast<IServerTarget>().ToArray()));
	}
}
