﻿using ABCo.Multicam.Server.Features.Switchers.Buffering;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public record class SpecsSpecificInfo(SwitcherSpecs Specs, IReadOnlyList<MixBlockState> State);
	public record struct MixBlockState(int Prog, int Prev);

	public enum SwitcherPlatformCompatibilityValue
	{
		Supported,
		UnsupportedPlatform,
		NoSoftware
	}

	public enum SwitcherConnectionStatus
	{
		NotConnected,
		Connecting,
		Connected
	}

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
		SwitcherPlatformCompatibilityValue PlatformCompatibility { get; }
		SwitcherConnectionStatus ConnectionStatus { get; }
		string? ErrorMessage { get; }
		SpecsSpecificInfo SpecsInfo { get; }
		SwitcherConfig Config { get; }

		void ChangeConfig(SwitcherConfig newConfig);
		void SetProgram(int mb, int val);
		void SetPreview(int mb, int val);
		void Cut(int param);
		void Connect();
		void Disconnect();
		void AcknowledgeError();
	}

	public partial class SwitcherFeature : BindableServerComponent<IFeature>, ISwitcherFeature, ISwitcherEventHandler
    {
		// The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
		readonly IHotSwappableSwitcherInteractionBuffer _buffer;

		[ObservableProperty] string _name = "New Switcher";
		[ObservableProperty] SwitcherPlatformCompatibilityValue _platformCompatibility = SwitcherPlatformCompatibilityValue.Supported;
		[ObservableProperty] SwitcherConnectionStatus _connectionStatus = SwitcherConnectionStatus.NotConnected;
		[ObservableProperty] string? _errorMessage = null;
		[ObservableProperty] SpecsSpecificInfo _specsInfo = new(new SwitcherSpecs(), Array.Empty<MixBlockState>());
		[ObservableProperty] SwitcherConfig _config = new DummySwitcherConfig(4);

		public FeatureTypes Type => FeatureTypes.Switcher;

		public SwitcherFeature(IServerInfo info) : base(info)
        {
            _buffer = info.Get<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(Config);
			_buffer.SetEventHandler(this);

			// Update the specs + connection to match the new ones
			SpecsInfo = new SpecsSpecificInfo(_buffer.CurrentBuffer.Specs, CreateMixBlockStateVals());
			OnConnectionStateChange(_buffer.CurrentBuffer.IsConnected);
		}

		public void Rename(string name) => Name = name;
		public void AcknowledgeError() => ErrorMessage = null;
		public void Connect()
		{
			ConnectionStatus = SwitcherConnectionStatus.Connecting;
			_buffer.CurrentBuffer.Connect();
		}

		public void Disconnect() => _buffer.CurrentBuffer.Disconnect();
		public void SetProgram(int mb, int val) => _buffer.CurrentBuffer.SendProgram(mb, val);
		public void SetPreview(int mb, int val) => _buffer.CurrentBuffer.SendPreview(mb, val);
		public void Cut(int param) => _buffer.CurrentBuffer.Cut(param);
		public void ChangeConfig(SwitcherConfig newConfig)
		{
			Config = newConfig;
			PlatformCompatibility = _buffer.CurrentBuffer.GetPlatformCompatibility(); // TODO: This should be more elegantly communicated from core.
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

        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => SpecsInfo = new SpecsSpecificInfo(SpecsInfo.Specs, CreateMixBlockStateVals());
		public void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => SpecsInfo = new SpecsSpecificInfo(SpecsInfo.Specs, CreateMixBlockStateVals());
		public void OnSpecsChange(SwitcherSpecs newSpecs) => SpecsInfo = new SpecsSpecificInfo(newSpecs, CreateMixBlockStateVals());
		public void OnConnectionStateChange(bool newState) => ConnectionStatus = newState ? SwitcherConnectionStatus.Connected : SwitcherConnectionStatus.NotConnected;

		public void OnFailure(SwitcherError error)
		{
            // Create a new buffer
            _buffer.ChangeSwitcher(Config);
			ErrorMessage = error.Message;
		}

		// Dispose:
		public override void DisposeComponent() => _buffer.Dispose();
	}
}