using ABCo.Multicam.Server.Features.Switchers.Buffering;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

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

    public interface ISwitcher : INotifyPropertyChanged, INamedServerComponent, IDisposable
    {
        // Data
        SwitcherPlatformCompatibilityValue PlatformCompatibility { get; }
        SwitcherConnectionStatus ConnectionStatus { get; }
        string? ErrorMessage { get; }
        SpecsSpecificInfo SpecsInfo { get; }
        SwitcherConfig Config { get; }

        // Actions
        void ChangeConfig(SwitcherConfig newConfig);
        void SetProgram(int mb, int val);
        void SetPreview(int mb, int val);
        void Cut(int param);
        void Connect();
        void Disconnect();
        void AcknowledgeError();
    }

    public partial class Switcher : BindableServerComponent<ISwitcher>, ISwitcher, ISwitcherEventHandler
    {
        // The buffer that sits atop the switcher to add preview/unsupported operation emulation, caching and more to all the switcher interactions.
        readonly IDynamicSwitcherBuffer _swapBuffer;

        [ObservableProperty] string _name = "New Switcher";
        [ObservableProperty] SwitcherPlatformCompatibilityValue _platformCompatibility = SwitcherPlatformCompatibilityValue.Supported;
        [ObservableProperty] SwitcherConnectionStatus _connectionStatus = SwitcherConnectionStatus.NotConnected;
        [ObservableProperty] string? _errorMessage = null;
        [ObservableProperty] SpecsSpecificInfo _specsInfo = new(new SwitcherSpecs(), Array.Empty<MixBlockState>());
        [ObservableProperty] SwitcherConfig _config = new OBSSwitcherConfig("192.168.0.19", 4455, "E1IEO45ZfkQzfy3f");

        public Switcher(IServerInfo info)
        {
            _swapBuffer = new DynamicSwitcherBuffer(Config, info);
            _swapBuffer.SetEventHandler(this);

            // Update the specs + connection to match the new ones
            SpecsInfo = new SpecsSpecificInfo(_swapBuffer.CurrentBuffer.Specs, CreateMixBlockStateVals(_swapBuffer.CurrentBuffer.Specs));
            OnConnectionStateChange(_swapBuffer.CurrentBuffer.IsConnected);
        }

        public void Rename(string name) => Name = name;
        public void AcknowledgeError() => ErrorMessage = null;
        public void Connect()
        {
            ConnectionStatus = SwitcherConnectionStatus.Connecting;
            _swapBuffer.CurrentBuffer.Connect();
        }

        public void Disconnect() => _swapBuffer.CurrentBuffer.Disconnect();
        public void SetProgram(int mb, int val) => _swapBuffer.CurrentBuffer.SendProgram(mb, val);
        public void SetPreview(int mb, int val) => _swapBuffer.CurrentBuffer.SendPreview(mb, val);
        public void Cut(int param) => _swapBuffer.CurrentBuffer.Cut(param);
        public void ChangeConfig(SwitcherConfig newConfig)
        {
            Config = newConfig;
            PlatformCompatibility = _swapBuffer.CurrentBuffer.GetPlatformCompatibility(); // TODO: This should be more elegantly communicated from the raw switcher.
            _swapBuffer.ChangeSwitcher(newConfig);
        }

        // Events:
        MixBlockState[] CreateMixBlockStateVals(SwitcherSpecs specs)
        {
            var res = new MixBlockState[specs.MixBlocks.Count];
            for (int i = 0; i < specs.MixBlocks.Count; i++)
                res[i] = new MixBlockState(_swapBuffer.CurrentBuffer.GetProgram(i), _swapBuffer.CurrentBuffer.GetPreview(i));

            return res;
        }

        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => SpecsInfo = new SpecsSpecificInfo(SpecsInfo.Specs, CreateMixBlockStateVals(_swapBuffer.CurrentBuffer.Specs));
        public void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => SpecsInfo = new SpecsSpecificInfo(SpecsInfo.Specs, CreateMixBlockStateVals(_swapBuffer.CurrentBuffer.Specs));
        public void OnSpecsChange(SwitcherSpecs newSpecs) => SpecsInfo = new SpecsSpecificInfo(newSpecs, CreateMixBlockStateVals(newSpecs));
        public void OnConnectionStateChange(bool newState) => ConnectionStatus = newState ? SwitcherConnectionStatus.Connected : SwitcherConnectionStatus.NotConnected;

        public void OnFailure(SwitcherError error)
        {
            // Create a new buffer
            _swapBuffer.ChangeSwitcher(Config);
            ErrorMessage = error.Message;
        }

        // Dispose:
        public void Dispose() => _swapBuffer.Dispose();
    }
}