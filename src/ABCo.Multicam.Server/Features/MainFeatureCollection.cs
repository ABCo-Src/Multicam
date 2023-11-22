using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Manages all the (running) features in the current project.
	/// </summary>
	public interface IMainFeatureCollection : IServerComponent, IDisposable
    {
        IClientNotifier<IMainFeatureCollectionState, IMainFeatureCollection> ClientNotifier { get; }

		void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

    public class MainFeatureCollection : IMainFeatureCollection
    {
        readonly IMainFeatureCollectionState _state;
		readonly IServerInfo _info;
        readonly List<IFeature> _features;

        public IClientNotifier<IMainFeatureCollectionState, IMainFeatureCollection> ClientNotifier => _state.ClientNotifier;

		public MainFeatureCollection(IServerInfo info)
        {
            _info = info;
            _state = info.Get<IMainFeatureCollectionState, IMainFeatureCollection>(this);
            _features = new();
			RefreshFeaturesList();
		}

        public void CreateFeature(FeatureTypes type)
        {
			_features.Add(_info.Get<IFeature, FeatureTypes>(type));
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
			_state.Dispose();

            for (int i = 0; i < _features.Count; i++)
                _features[i].Dispose();
        }

        void RefreshFeaturesList() => _state.Features = _features.Select(f => f.State).ToArray();
	}
}
