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
        IBinderForProjectFeatures VMBinder { get; }

        void CreateFeature(FeatureTypes type);
        void MoveUp(IRunningFeature feature);
        void MoveDown(IRunningFeature feature);
        void Delete(IRunningFeature feature);
    }

    public interface IBinderForProjectFeatures
    {
        void ModelChange_FeaturesChange();
        void FinishConstruction(IFeatureManager model);
    }

    public class FeatureManager : IFeatureManager
    {
        IServiceSource _servSource;
        List<IRunningFeature> _runningFeatures = new();

        public IReadOnlyList<IRunningFeature> Features => _runningFeatures;
        public IBinderForProjectFeatures VMBinder { get; private set; }

        public FeatureManager(IServiceSource source, IBinderForProjectFeatures binder)
        {
            VMBinder = binder;
            binder.FinishConstruction(this);

            _servSource = source;
        }

        public void CreateFeature(FeatureTypes type)
        {
            _runningFeatures.Add(GetFeatureFromType(type));
            VMBinder.ModelChange_FeaturesChange();
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

            VMBinder.ModelChange_FeaturesChange();
        }

        public void MoveDown(IRunningFeature feature)
        {
            int indexOfFeature = _runningFeatures.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _runningFeatures.Count - 1) return;

            (_runningFeatures[indexOfFeature], _runningFeatures[indexOfFeature + 1]) = (_runningFeatures[indexOfFeature + 1], _runningFeatures[indexOfFeature]);

            VMBinder.ModelChange_FeaturesChange();
        }

        public void Delete(IRunningFeature feature)
        {
            _runningFeatures.Remove(feature);
            feature.Dispose();

            VMBinder.ModelChange_FeaturesChange();
        }

        public void Dispose()
        {
            for (int i = 0; i < _runningFeatures.Count; i++)
                _runningFeatures[i].Dispose();
        }
    }
}
