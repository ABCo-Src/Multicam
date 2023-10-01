namespace ABCo.Multicam.Server.Features.Switchers.Interaction
{
	public interface IPerSpecSwitcherInteractionBuffer : IServerService<SwitcherSpecs, ISwitcher>
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

    public class PerSpecSwitcherInteractionBuffer : IPerSpecSwitcherInteractionBuffer
    {
        readonly ISwitcherInteractionBufferFactory _factory;

		readonly ISwitcher _rawSwitcher;
		readonly IMixBlockInteractionBuffer[] _mixBlockBuffers;

        public SwitcherSpecs Specs { get; private set; }

		public PerSpecSwitcherInteractionBuffer(SwitcherSpecs specs, ISwitcher switcher, IServerInfo servSource)
		{
			_factory = servSource.Get<ISwitcherInteractionBufferFactory>();

			Specs = specs;
			_rawSwitcher = switcher;

			// Create mix block buffers
			_mixBlockBuffers = new IMixBlockInteractionBuffer[Specs.MixBlocks.Count];
			for (int i = 0; i < Specs.MixBlocks.Count; i++)
				_mixBlockBuffers[i] = _factory.CreateMixBlock(Specs.MixBlocks[i], i, switcher);
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
