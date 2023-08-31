using ABCo.Multicam.Core.Features.Switchers.Fading;
using System;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IPerSpecSwitcherInteractionBuffer : INeedsInitialization<SwitcherSpecs, ISwitcher>, ISwitcherEventHandler
    {
        bool IsConnected { get; }
        SwitcherSpecs Specs { get; }
        void SetEventHandler(ISwitcherEventHandler? eventHandler);
        int GetProgram(int mixBlock);
        int GetPreview(int mixBlock);
        void SendProgram(int mixBlock, int value);
        void SendPreview(int mixBlock, int value);
        void Cut(int mixBlock);
        void DisposeSwitcher();
    }

    public class PerSpecSwitcherInteractionBuffer : IPerSpecSwitcherInteractionBuffer
    {
        readonly ISwitcherInteractionBufferFactory _factory;
        
        ISwitcherEventHandler? _eventHandler;
        ISwitcher _rawSwitcher = null!;
        IMixBlockInteractionBuffer[] _mixBlockBuffers = null!;

        public bool IsConnected { get; private set; }
        public SwitcherSpecs Specs { get; private set; } = null!;

		public PerSpecSwitcherInteractionBuffer(ISwitcherInteractionBufferFactory factory) => _factory = factory;

		public void FinishConstruction(SwitcherSpecs specs, ISwitcher switcher)
        {
            Specs = specs;
            _rawSwitcher = switcher;
            switcher.SetEventHandler(this);

            if (switcher.IsConnected)
            {
                IsConnected = true;

                // Create mix block buffers
                _mixBlockBuffers = new IMixBlockInteractionBuffer[Specs.MixBlocks.Count];
                for (int i = 0; i < Specs.MixBlocks.Count; i++)
                {
                    _mixBlockBuffers[i] = _factory.CreateMixBlock(Specs.MixBlocks[i], i, switcher);
                    _mixBlockBuffers[i].RefreshValues();
                }
            }
            else
            {
                _mixBlockBuffers = Array.Empty<IMixBlockInteractionBuffer>();
                IsConnected = false;
            }
        }

		public void SetEventHandler(ISwitcherEventHandler? eventHandler)
		{
			_eventHandler = eventHandler;
            for (int i = 0; i < _mixBlockBuffers.Length; i++)
                _mixBlockBuffers[i].SetEventHandler(eventHandler);
		}

		public int GetProgram(int mixBlock) => _mixBlockBuffers[mixBlock].Program;
        public int GetPreview(int mixBlock) => _mixBlockBuffers[mixBlock].Preview;

        public void SendProgram(int mixBlock, int value)
        {
            if (!IsConnected) return;
            _mixBlockBuffers[mixBlock].SendProgram(value);
        }

        public void SendPreview(int mixBlock, int value)
        {
            if (!IsConnected) return;
            _mixBlockBuffers[mixBlock].SendPreview(value);
        }

        public void OnProgramChangeFinish(SwitcherProgramChangeInfo info)
        {
            _mixBlockBuffers[info.MixBlock].UpdateProg(info.NewValue);
            _eventHandler?.OnProgramChangeFinish(info);
        }

        public void OnPreviewChangeFinish(SwitcherPreviewChangeInfo info)
        {
            _mixBlockBuffers[info.MixBlock].UpdatePrev(info.NewValue);
            _eventHandler?.OnPreviewChangeFinish(info);
        }

        public void OnSpecsChange(SwitcherSpecs newSpecs) => _eventHandler?.OnSpecsChange(newSpecs);

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
