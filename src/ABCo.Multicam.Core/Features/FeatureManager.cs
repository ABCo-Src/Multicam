namespace ABCo.Multicam.Core.Features
{
	/// <summary>
	/// Manages all the (running) features in the current project.
	/// </summary>
	public interface IFeatureManager : IDisposable
    {
        IReadOnlyList<IFeature> Features { get; }
		IProjectFeaturesPresenter UIPresenter { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

	public interface IProjectFeaturesPresenter : IParameteredService<IFeatureManager>
    {
        void OnItemsChange();
    }

    public class FeatureManager : IFeatureManager
    {
        readonly IServiceSource _servSource;
        readonly List<IFeature> _runningFeatures = new();

        public IReadOnlyList<IFeature> Features => _runningFeatures;
        public IProjectFeaturesPresenter UIPresenter { get; private set; }

        public FeatureManager(IServiceSource source)
        {
            _servSource = source;
            UIPresenter = source.Get<IProjectFeaturesPresenter, IFeatureManager>(this);
        }

        public void CreateFeature(FeatureTypes type)
        {
            _runningFeatures.Add(_servSource.Get<IFeature, FeatureTypes>(type));
            UIPresenter.OnItemsChange();
        }

        public void MoveUp(IFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature - 1]) = (_runningFeatures[indexOfFeature - 1], _runningFeatures[indexOfFeature]);

            UIPresenter.OnItemsChange();
        }

        public void MoveDown(IFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _runningFeatures.Count - 1) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature + 1]) = (_runningFeatures[indexOfFeature + 1], _runningFeatures[indexOfFeature]);

            UIPresenter.OnItemsChange();
        }

        public void Delete(IFeature feature)
        {
            _runningFeatures.Remove(feature);
            feature.Dispose();

            UIPresenter.OnItemsChange();
        }

        public void Dispose()
        {
            for (int i = 0; i < _runningFeatures.Count; i++)
                _runningFeatures[i].Dispose();
        }
    }
}
