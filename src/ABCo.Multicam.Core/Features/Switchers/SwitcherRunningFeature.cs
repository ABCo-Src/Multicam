using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : ILiveFeature
    {
        SwitcherSpecs SwitcherSpecs { get; }
        int GetProgram(int mixBlock);
        int GetPreview(int mixBlock);
        void SendProgram(int mixBlock, int value);
        void SendPreview(int mixBlock, int value);
        void Cut(int mixBlock);
    }

    public interface ISwitcherEventHandler
    {
        void OnProgramChangeFinish(SwitcherProgramChangeInfo info);
        void OnPreviewChangeFinish(SwitcherPreviewChangeInfo info);
        void OnSpecsChange(SwitcherSpecs newSpecs);
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
        readonly IDynamicSwitcherInteractionBuffer _buffer;
        readonly IBinderForSwitcherFeature _uiBinder;

        public SwitcherRunningFeature(IServiceSource serviceSource)
        {
            _buffer = serviceSource.Get<IDynamicSwitcherInteractionBuffer, ISwitcherEventHandler>(this);
            _uiBinder = serviceSource.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(this);
            _buffer.ChangeSwitcher(new DummySwitcherConfig(4));
        }

        // Properties:
        public FeatureTypes FeatureType => FeatureTypes.Switcher;
        public ILiveFeatureBinder UIBinder => _uiBinder;
        public bool IsConnected => _buffer.CurrentBuffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _buffer.CurrentBuffer.Specs;

        // Methods:
        public int GetProgram(int mixBlock) => _buffer.CurrentBuffer.GetProgram(mixBlock);
        public int GetPreview(int mixBlock) => _buffer.CurrentBuffer.GetPreview(mixBlock);
        public void SendProgram(int mixBlock, int value) => _buffer.CurrentBuffer.SendProgram(mixBlock, value);
        public void SendPreview(int mixBlock, int value) => _buffer.CurrentBuffer.SendPreview(mixBlock, value);
        public void ChangeSwitcher(SwitcherConfig config) => _buffer.ChangeSwitcher(config);
        public void Cut(int mixBlock) => _buffer.CurrentBuffer.Cut(mixBlock);

        // Events:
        public void OnProgramChangeFinish(SwitcherProgramChangeInfo info) => _uiBinder.ModelChange_BusValues();
        public void OnPreviewChangeFinish(SwitcherPreviewChangeInfo info) => _uiBinder.ModelChange_BusValues();
        public void OnSpecsChange(SwitcherSpecs newSpecs) => _uiBinder.ModelChange_Specs();

        // Dispose:
        public void Dispose() => _buffer.Dispose();

    }
}
