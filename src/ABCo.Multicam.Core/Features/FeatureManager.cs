namespace ABCo.Multicam.Core.Features
{
	/// <summary>
	/// Manages all the (running) features in the current project.
	/// </summary>
	public interface IFeatureManager : IDisposable
    {
        IReadOnlyList<IFeature> Features { get; }
        IBinderForProjectFeatures UIBinder { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

    public interface IBinderForProjectFeatures : INeedsInitialization<IFeatureManager>
    {
        void ModelChange_FeaturesChange();
    }

    public class FeatureManager : IFeatureManager
    {
        readonly IServiceSource _servSource;
        readonly List<IFeature> _runningFeatures = new();

        public IReadOnlyList<IFeature> Features => _runningFeatures;
        public IBinderForProjectFeatures UIBinder { get; private set; }

        public FeatureManager(IServiceSource source)
        {
            _servSource = source;
            UIBinder = source.Get<IBinderForProjectFeatures, IFeatureManager>(this);
        }

        public void CreateFeature(FeatureTypes type)
        {
            _runningFeatures.Add(_servSource.Get<IFeature, FeatureTypes>(type));
            UIBinder.ModelChange_FeaturesChange();
        }

        public void MoveUp(IFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature - 1]) = (_runningFeatures[indexOfFeature - 1], _runningFeatures[indexOfFeature]);

            UIBinder.ModelChange_FeaturesChange();
        }

        public void MoveDown(IFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _runningFeatures.Count - 1) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature + 1]) = (_runningFeatures[indexOfFeature + 1], _runningFeatures[indexOfFeature]);

            UIBinder.ModelChange_FeaturesChange();
        }

        public void Delete(IFeature feature)
        {
            _runningFeatures.Remove(feature);
            feature.Dispose();

            UIBinder.ModelChange_FeaturesChange();
        }

        public void Dispose()
        {
            for (int i = 0; i < _runningFeatures.Count; i++)
                _runningFeatures[i].Dispose();
        }
    }
}
