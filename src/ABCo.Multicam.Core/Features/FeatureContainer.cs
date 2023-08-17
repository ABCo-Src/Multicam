using ABCo.Multicam.Core.Features.Switchers;

namespace ABCo.Multicam.Core.Features
{
    public interface IBinderForFeatureContainer : INeedsInitialization<IFeatureManager, IFeatureContainer>
    {
    }

    /// <summary>
    /// Represents a feature currently loaded, either on this system or another system.
    /// Introduces properties shared across all features, such as titles or machine switching.
    /// </summary>
    public interface IFeatureContainer : INeedsInitialization<FeatureTypes>, IDisposable
    {
        IBinderForFeatureContainer UIBinder { get; }
        ILiveFeature CurrentFeature { get; }
    }

    public class FeatureContainer : IFeatureContainer
    {
        readonly IServiceSource _servSource;
        readonly IFeatureManager _manager;

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
