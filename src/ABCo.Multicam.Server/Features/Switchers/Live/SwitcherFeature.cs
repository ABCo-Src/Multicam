using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Interaction;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public interface ISwitcherEventHandler
    {
        void OnProgramValueChange(SwitcherProgramChangeInfo info);
        void OnPreviewValueChange(SwitcherPreviewChangeInfo info);
        void OnSpecsChange(SwitcherSpecs newSpecs);
        void OnConnectionStateChange(bool isConnected);
        void OnFailure(SwitcherError error);
    }

    public interface ISwitcherFeature : IFeature 
	{
		void ChangeConfig(SwitcherConfig newConfig);
		void SetProgram(int mb, int val);
		void SetPreview(int mb, int val);
		void Cut(int param);
		void Connect();
		void Disconnect();
		void AcknowledgeError();
	}

	public class SwitcherFeature : ISwitcherFeature, ISwitcherEventHandler
    {
		// The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
		readonly IHotSwappableSwitcherInteractionBuffer _buffer;
		readonly ISwitcherFeatureState _state;

		public IFeatureState State => _state;

		public SwitcherFeature(IServerInfo info)
        {
			_state = info.Get<ISwitcherFeatureState, IFeature>(this);
            _buffer = info.Get<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(_state.Config);
			_buffer.SetEventHandler(this);

			// Update the specs + connection to match the new ones
			_state.SpecsInfo = new SpecsSpecificInfo(new SwitcherSpecs(), CreateMixBlockStateVals());
			_state.IsConnected = _buffer.CurrentBuffer.IsConnected;
		}

		public void Rename(string name) => _state.Name = name;
		public void AcknowledgeError() => _state.ErrorMessage = null;
		public void Connect() => _buffer.CurrentBuffer.Connect();
		public void Disconnect() => _buffer.CurrentBuffer.Disconnect();
		public void SetProgram(int mb, int val) => _buffer.CurrentBuffer.SendProgram(mb, val);
		public void SetPreview(int mb, int val) => _buffer.CurrentBuffer.SendPreview(mb, val);
		public void Cut(int param) => _buffer.CurrentBuffer.Cut(param);
		public void ChangeConfig(SwitcherConfig newConfig)
		{
			_state.Config = newConfig;
			_state.PlatformCompatibility = _buffer.CurrentBuffer.GetPlatformCompatibility(); // TODO: This should be more elegantly communicated from core.
			_buffer.ChangeSwitcher(newConfig);
		}

		// Events:
		MixBlockState[] CreateMixBlockStateVals()
        {
            var specs = _buffer.CurrentBuffer.Specs;

            var res = new MixBlockState[specs.MixBlocks.Count];
            for (int i = 0; i < specs.MixBlocks.Count; i++)
                res[i] = new MixBlockState(_buffer.CurrentBuffer.GetProgram(i), _buffer.CurrentBuffer.GetPreview(i));

			return res;
        }

        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => CreateMixBlockStateVals();
		public void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => CreateMixBlockStateVals();
		public void OnSpecsChange(SwitcherSpecs newSpecs) => _state.SpecsInfo = new SpecsSpecificInfo(newSpecs, CreateMixBlockStateVals());
		public void OnConnectionStateChange(bool newState) => _state.IsConnected = newState;

		public void OnFailure(SwitcherError error)
		{
            // Create a new buffer
            _buffer.ChangeSwitcher(_state.Config);
			_state.ErrorMessage = error.Message;
		}

		// Dispose:
		public void Dispose() => _buffer.Dispose();
	}
}