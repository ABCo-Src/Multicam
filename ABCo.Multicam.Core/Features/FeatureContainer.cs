using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
    public interface IBinderForFeatureContainer // IVMBinder<IVMForFeatureBinder>
    {
        void FinishConstruction(IFeatureManager manager, IFeatureContainer feature);
    }

    /// <summary>
    /// Represents a feature currently loaded, either on this system or another system.
    /// Introduces properties shared across all features, such as titles or machine switching.
    /// </summary>
    public interface IFeatureContainer : IDisposable
    {
        IBinderForFeatureContainer UIBinder { get; }
        ILiveFeature CurrentFeature { get; }
        void FinishConstruction(FeatureTypes featureType);
    }

    public class FeatureContainer : IFeatureContainer
    {
        IServiceSource _servSource;
        IFeatureManager _manager;

        public IBinderForFeatureContainer UIBinder { get; }
        public ILiveFeature CurrentFeature { get; private set; } = null!;

        public FeatureContainer(IBinderForFeatureContainer binder, IServiceSource servSource, IFeatureManager manager)
        {
            _servSource = servSource;
            _manager = manager;
            UIBinder = binder;
        }

        public void FinishConstruction(FeatureTypes featureType)
        {
            CurrentFeature = featureType switch
            {
                FeatureTypes.Switcher => _servSource.Get<ISwitcherRunningFeature>(),
                _ => _servSource.Get<IUnsupportedRunningFeature>(),
            };

            UIBinder.FinishConstruction(_manager, this);
        }

        public void Dispose() => CurrentFeature.Dispose();
    }
}
