using ABCo.Multicam.Core.Features.Switchers.Fading;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface ISwitcherInteractionBuffer : INeedsInitialization<ISwitcher>
    {
        bool IsConnected { get; }
        SwitcherSpecs Specs { get; }
        int GetValue(int mixBlock, int bus);
        void PostValue(int mixBlock, int bus, int value);
        void Cut(int mixBlock);
        void SetOnBusChangeFinishCall(Action<RetrospectiveFadeInfo?>? callback);
        void DisposeSwitcher();
    }

    public class SwitcherInteractionBuffer : ISwitcherInteractionBuffer
    {
        readonly ISwitcherInteractionBufferFactory _factory;

        ISwitcher _rawSwitcher = null!;
        IMixBlockInteractionBuffer[] _mixBlockBuffers = null!;
        Action<RetrospectiveFadeInfo?>? _onBusChangeFinishCall;

        public bool IsConnected { get; private set; }
        public SwitcherSpecs Specs { get; private set; } = null!;

        public SwitcherInteractionBuffer(ISwitcherInteractionBufferFactory factory)
        {
            _factory = factory;
        }
        
        public void FinishConstruction(ISwitcher switcher)
        {
            _rawSwitcher = switcher;

            if (switcher.IsConnected)
            {
                Specs = switcher.ReceiveSpecs();
                IsConnected = true;

                // Create mix block buffers
                _mixBlockBuffers = new IMixBlockInteractionBuffer[Specs.MixBlocks.Count];
                for (int i = 0; i < Specs.MixBlocks.Count; i++)
                {
                    _mixBlockBuffers[i] = _factory.CreateMixBlock(Specs.MixBlocks[i], i, switcher);
                    _mixBlockBuffers[i].SetCacheChangeExceptRefreshCall(OnCacheChange);
                }

                // Assign bus change event handler
                _rawSwitcher.SetOnBusChangeFinishCall(OnBusChange);
            }
            else
            {
                _mixBlockBuffers = Array.Empty<IMixBlockInteractionBuffer>();
                Specs = new();
                IsConnected = false;
            }
        }

        public int GetValue(int mixBlock, int bus) => bus == 0 ? _mixBlockBuffers[mixBlock].Program : _mixBlockBuffers[mixBlock].Preview;

        public void PostValue(int mixBlock, int bus, int value)
        {
            if (!IsConnected) return;

            if (bus == 0)
                _mixBlockBuffers[mixBlock].SetProgram(value);
            else
                _mixBlockBuffers[mixBlock].SetPreview(value);
        }

        void OnBusChange(SwitcherBusChangeInfo info)
        {
            if (info.IsBusKnown)
            {
                if (info.Bus == 0)
                    _mixBlockBuffers[info.MixBlock].RefreshWithKnownProg(info.NewValue);
                else
                    _mixBlockBuffers[info.MixBlock].RefreshWithKnownPrev(info.NewValue);
            }
            else
            {
                for (int i = 0; i < Specs.MixBlocks.Count; i++)
                    _mixBlockBuffers[i].RefreshCache();
            }

            _onBusChangeFinishCall?.Invoke(info.FadeInfo);
        }

        void OnCacheChange(RetrospectiveFadeInfo info) => _onBusChangeFinishCall?.Invoke(info);
        public void SetOnBusChangeFinishCall(Action<RetrospectiveFadeInfo?>? callback) => _onBusChangeFinishCall = callback;

        public void Cut(int mixBlock) => _mixBlockBuffers[mixBlock].Cut();
        public void DisposeSwitcher() => _rawSwitcher.Dispose();

        record struct MixBlockStore(int Program, int Preview);
    }

    public interface ISwitcherInteractionBufferFactory
    {
        IMixBlockInteractionBuffer CreateMixBlock(SwitcherMixBlock mixBlock, int mixBlockIdx, ISwitcher switcher);
        IMixBlockInteractionEmulator CreateMixBlockEmulator(SwitcherMixBlock mixBlock, int mixBlockIdx, ISwitcher switcher, IMixBlockInteractionBuffer parentBuffer);
    }

    public class SwitcherInteractionBufferFactory : ISwitcherInteractionBufferFactory
    {
        //public ISwitcherInteractionBuffer CreateSync(ISwitcher switcher) => new SwitcherInteractionBuffer(switcher, this);
        //public async Task<ISwitcherInteractionBuffer> CreateAsync(ISwitcher switcher) => await Task.Run(() => CreateSync(switcher));

        public IMixBlockInteractionBuffer CreateMixBlock(SwitcherMixBlock mixBlock, int mixBlockIdx, ISwitcher switcher) =>
            new MixBlockInteractionBuffer(mixBlock, mixBlockIdx, switcher, this);

        public IMixBlockInteractionEmulator CreateMixBlockEmulator(SwitcherMixBlock mixBlock, int mixBlockIdx, ISwitcher switcher, IMixBlockInteractionBuffer parentBuffer) =>
            new MixBlockInteractionEmulator(mixBlock, mixBlockIdx, switcher, parentBuffer);
    }
}
