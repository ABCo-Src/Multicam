using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : ILiveFeature
    {
        SwitcherSpecs SwitcherSpecs { get; }
        int GetValue(int mixBlock, int bus);
        void PostValue(int mixBlock, int bus, int value);
        void Cut(int mixBlock);
    }

    public interface ISwitcherEventHandler
    {
        void OnBusChangeFinish(RetrospectiveFadeInfo? info);
    }

    public interface IBinderForSwitcherFeature : ILiveFeatureBinder, INeedsInitialization<ISwitcherRunningFeature>
    {
        void ModelChange_Specs();
        void ModelChange_BusValues();
    }

    public class SwitcherRunningFeature : ISwitcherRunningFeature, ISwitcherEventHandler
    {
        // TODO: Add slamming protection
        // TODO: Add error handling

        // The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
        // A new interaction buffer is created anytime the specs change, and the swapper facilitates for us.
        readonly ISwitcherSwapper _swapper;
        readonly IBinderForSwitcherFeature _uiBinder;

        public SwitcherRunningFeature(IServiceSource serviceSource)
        {
            _swapper = serviceSource.Get<ISwitcherSwapper, ISwitcherEventHandler>(this);
            _uiBinder = serviceSource.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(this);
        }

        // Properties:
        public FeatureTypes FeatureType => FeatureTypes.Switcher;
        public ILiveFeatureBinder UIBinder => _uiBinder;
        public bool IsConnected => _swapper.CurrentBuffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _swapper.CurrentBuffer.Specs;

        // Methods:
        public int GetValue(int mixBlock, int bus) => _swapper.CurrentBuffer.GetValue(mixBlock, bus);
        public void PostValue(int mixBlock, int bus, int value) => _swapper.CurrentBuffer.PostValue(mixBlock, bus, value);
        public void ChangeSwitcher(SwitcherConfig config) => _swapper.ChangeSwitcher(config);
        public void Cut(int mixBlock) => _swapper.CurrentBuffer.Cut(mixBlock);

        // Events:
        public void OnBusChangeFinish(RetrospectiveFadeInfo? info) => _uiBinder.ModelChange_BusValues();

        // Dispose:
        public void Dispose() => _swapper.Dispose();
    }
}
