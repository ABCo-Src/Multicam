using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
    /// <summary>
    /// Represents a feature currently loaded, either on this system or another system.
    /// Introduces properties shared across all features, such as titles or machine switching.
    /// </summary>
    public interface IFeatureContainer : IDisposable
    {
        void FinishConstruction(FeatureTypes featureType);
    }

    public class FeatureContainer : IFeatureContainer
    {
        IServiceSource _servSource;
        ILiveFeature _feature = null!;

        public FeatureContainer(IServiceSource servSource) => _servSource = servSource;
        public void FinishConstruction(FeatureTypes featureType)
        {
            _feature = featureType switch
            {
                FeatureTypes.Switcher => _servSource.Get<ISwitcherRunningFeature>(),
                _ => _servSource.Get<IUnsupportedRunningFeature>(),
            };
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
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
