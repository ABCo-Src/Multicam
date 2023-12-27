using ABCo.Multicam.Server.Features.Switchers.Core;

namespace ABCo.Multicam.Server.Features.Switchers.Buffering
{
	public interface IPerSpecSwitcherInteractionBuffer : IServerService<SwitcherSpecs, IRawSwitcher>
    {
        SwitcherSpecs Specs { get; }
        void SetEventHandler(ISwitcherEventHandler? eventHandler);
        int GetProgram(int mixBlock);
        int GetPreview(int mixBlock);
        void SendProgram(int mixBlock, int value);
        void SendPreview(int mixBlock, int value);
        void UpdateProg(SwitcherProgramChangeInfo info);
        void UpdatePrev(SwitcherPreviewChangeInfo info);
        void UpdateEverything();
        void Cut(int mixBlock);
    }

    public class PerSpecSwitcherBuffer : IPerSpecSwitcherInteractionBuffer
    {
        readonly IRawSwitcher _rawSwitcher;
        readonly IMixBlockBuffer[] _mixBlockBuffers;

        public SwitcherSpecs Specs { get; private set; }

        public PerSpecSwitcherBuffer(SwitcherSpecs specs, IRawSwitcher switcher)
        {
            Specs = specs;
            _rawSwitcher = switcher;

            // Create mix block buffers
            _mixBlockBuffers = new IMixBlockBuffer[Specs.MixBlocks.Count];
            for (int i = 0; i < Specs.MixBlocks.Count; i++)
                _mixBlockBuffers[i] = new MixBlockBuffer(Specs.MixBlocks[i], i, switcher);
        }

        public void UpdateEverything()
        {
            for (int i = 0; i < Specs.MixBlocks.Count; i++)
                _mixBlockBuffers[i].RefreshValues();
        }

        public void SetEventHandler(ISwitcherEventHandler? eventHandler)
        {
            for (int i = 0; i < _mixBlockBuffers.Length; i++)
                _mixBlockBuffers[i].SetEventHandler(eventHandler);
        }

        public int GetProgram(int mixBlock) => _mixBlockBuffers[mixBlock].Program;
        public int GetPreview(int mixBlock) => _mixBlockBuffers[mixBlock].Preview;
        public void SendProgram(int mixBlock, int value) => _mixBlockBuffers[mixBlock].SendProgram(value);
        public void SendPreview(int mixBlock, int value) => _mixBlockBuffers[mixBlock].SendPreview(value);
        public void UpdateProg(SwitcherProgramChangeInfo info) => _mixBlockBuffers[info.MixBlock].UpdateProg(info.NewValue);
        public void UpdatePrev(SwitcherPreviewChangeInfo info) => _mixBlockBuffers[info.MixBlock].UpdatePrev(info.NewValue);

        public void Cut(int mixBlock) => _mixBlockBuffers[mixBlock].Cut();
    }
}
