namespace ABCo.Multicam.Core.Features
{
    /// <summary>
    /// Manages all the (running) features in the current project.
    /// </summary>
    public interface IFeatureManager : IDisposable
    {
        IReadOnlyList<IFeatureContainer> Features { get; }
        IBinderForProjectFeatures UIBinder { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IFeatureContainer feature);
        void MoveDown(IFeatureContainer feature);
        void Delete(IFeatureContainer feature);
    }

    public interface IBinderForProjectFeatures : INeedsInitialization<IFeatureManager>
    {
        void ModelChange_FeaturesChange();
    }

    public class FeatureManager : IFeatureManager
    {
        readonly IServiceSource _servSource;
        readonly List<IFeatureContainer> _runningFeatures = new();

        public IReadOnlyList<IFeatureContainer> Features => _runningFeatures;
        public IBinderForProjectFeatures UIBinder { get; private set; }

        public FeatureManager(IServiceSource source, IBinderForProjectFeatures binder)
        {
            UIBinder = binder;
            binder.FinishConstruction(this);

            _servSource = source;
        }

        public void CreateFeature(FeatureTypes type)
        {
            var newContainer = _servSource.Get<IFeatureContainer>();
            newContainer.FinishConstruction(type);
            _runningFeatures.Add(newContainer);

            UIBinder.ModelChange_FeaturesChange();
        }

        public void MoveUp(IFeatureContainer feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature - 1]) = (_runningFeatures[indexOfFeature - 1], _runningFeatures[indexOfFeature]);

            UIBinder.ModelChange_FeaturesChange();
        }

        public void MoveDown(IFeatureContainer feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _runningFeatures.Count - 1) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature + 1]) = (_runningFeatures[indexOfFeature + 1], _runningFeatures[indexOfFeature]);

            UIBinder.ModelChange_FeaturesChange();
        }

        public void Delete(IFeatureContainer feature)
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
