using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
    /// <summary>
    /// Manages all the (running) features in the current project.
    /// </summary>
    public interface IFeatureManager : IDisposable
    {
        IReadOnlyList<IRunningFeature> Features { get; }

        void CreateFeature(FeatureTypes type);
        void SetOnFeaturesChangeForVM(Action act);
        void MoveUp(IRunningFeature feature);
        void MoveDown(IRunningFeature feature);
        void Delete(IRunningFeature feature);
    }

    public interface IBinderForProjectFeatures
    {
        void ModelChange_FeaturesChange();
    }

    public class FeatureManager : IFeatureManager
    {
        IServiceSource _servSource;
        Action? _onFeaturesChange;
        List<IRunningFeature> _runningFeatures = new();

        public IReadOnlyList<IRunningFeature> Features => _runningFeatures;

        public FeatureManager(IServiceSource source) => _servSource = source;

        public void SetOnFeaturesChangeForVM(Action act) => _onFeaturesChange = act;

        public void CreateFeature(FeatureTypes type)
        {
            _runningFeatures.Add(GetFeatureFromType(type));
            _onFeaturesChange?.Invoke();
        }

        IRunningFeature GetFeatureFromType(FeatureTypes type)
        {
            return type switch
            {
                FeatureTypes.Switcher => _servSource.Get<ISwitcherRunningFeature>(),
                _ => _servSource.Get<IUnsupportedRunningFeature>()
            };
        }

        public void MoveUp(IRunningFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature - 1]) = (_runningFeatures[indexOfFeature - 1], _runningFeatures[indexOfFeature]);

            _onFeaturesChange?.Invoke();
        }

        public void MoveDown(IRunningFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _runningFeatures.Count - 1) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature + 1]) = (_runningFeatures[indexOfFeature + 1], _runningFeatures[indexOfFeature]);

            _onFeaturesChange?.Invoke();
        }

        public void Delete(IRunningFeature feature)
        {
            _runningFeatures.Remove(feature);
            feature.Dispose();
            _onFeaturesChange?.Invoke();
        }

        public void Dispose()
        {
            for (int i = 0; i < _runningFeatures.Count; i++)
                _runningFeatures[i].Dispose();
        }
    }
}
