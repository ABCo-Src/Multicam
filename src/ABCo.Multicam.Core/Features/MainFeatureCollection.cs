﻿using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Interaction;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Core.Features
{
	/// <summary>
	/// Manages all the (running) features in the current project.
	/// </summary>
	public interface IMainFeatureCollection : IDisposable
    {
        IReadOnlyList<IFeature> Features { get; }
		IProjectFeaturesPresenter UIPresenter { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

	public interface IProjectFeaturesPresenter : IParameteredService<IMainFeatureCollection, IScopeInfo>
    {
        void OnItemsChange();
    }

    public class MainFeatureCollection : IMainFeatureCollection
    {
        readonly IServiceSource _servSource;
        readonly IFeatureContentFactory _featureContentFactory;
        readonly List<IFeature> _features = new();

        public IReadOnlyList<IFeature> Features => _features;
        public IProjectFeaturesPresenter UIPresenter { get; private set; }

        public MainFeatureCollection(IServiceSource source)
        {
            _servSource = source;
            _featureContentFactory = source.Get<IFeatureContentFactory>();
			UIPresenter = source.Get<IProjectFeaturesPresenter, IMainFeatureCollection, IScopeInfo>(this, _servSource.Get<IScopedConnectionManager>().CreateScope());
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
            UIPresenter.OnItemsChange();
        }

        public void MoveUp(IFeature feature)
        {
            int indexOfFeature = _features.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_features[indexOfFeature], _features[indexOfFeature - 1]) = (_features[indexOfFeature - 1], _features[indexOfFeature]);

            UIPresenter.OnItemsChange();
        }

        public void MoveDown(IFeature feature)
        {
            int indexOfFeature = _features.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _features.Count - 1) return;

            (_features[indexOfFeature], _features[indexOfFeature + 1]) = (_features[indexOfFeature + 1], _features[indexOfFeature]);

            UIPresenter.OnItemsChange();
        }

        public void Delete(IFeature feature)
        {
            _features.Remove(feature);
            feature.Dispose();

            UIPresenter.OnItemsChange();
        }

        public void Dispose()
        {
            for (int i = 0; i < _features.Count; i++)
                _features[i].Dispose();
        }
    }
}
