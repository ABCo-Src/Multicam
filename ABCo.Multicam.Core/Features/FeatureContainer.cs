using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        void FinishConstruction(FeatureTypes featureType);
    }

    public class FeatureContainer : IFeatureContainer
    {
        IServiceSource _servSource;
        ILiveFeature _feature = null!;

        public IBinderForFeatureContainer UIBinder { get; }

        public FeatureContainer(IBinderForFeatureContainer binder, IServiceSource servSource, IFeatureManager manager)
        {
            _servSource = servSource;
            UIBinder = binder;

            binder.FinishConstruction(manager, this);
        }

        public void FinishConstruction(FeatureTypes featureType)
        {
            _feature = featureType switch
            {
                FeatureTypes.Switcher => _servSource.Get<ISwitcherRunningFeature>(),
                _ => _servSource.Get<IUnsupportedRunningFeature>(),
            };
        }

        public void Dispose() => _feature.Dispose();
    }

    public interface IUnsupportedRunningFeature : ILiveFeature { }

    public class UnsupportedRunningFeature : IUnsupportedRunningFeature
    {
        public void Dispose() { }

        public void FinishConstruction(FeatureTypes featureType)
        {
            throw new NotImplementedException();
        }
    }
}
