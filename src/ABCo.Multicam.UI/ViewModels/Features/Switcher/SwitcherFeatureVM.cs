using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	using AlsoNotify = NotifyPropertyChangedForAttribute;

	public interface ISwitcherFeatureVM : IVMForSwitcherFeature, ILiveFeatureViewModel
    {
		void UpdateConfig(SwitcherConfig config);
	}

    public partial class SwitcherFeatureVM : BindingViewModelBase<IVMForSwitcherFeature>, ISwitcherFeatureVM
    {
        IServiceSource _servSource;

        // Synced to the model:
        [ObservableProperty] ISwitcherRunningFeature _rawFeature = null!;
        [ObservableProperty] IVMBinder<IVMForSwitcherMixBlock>[] _rawMixBlocks = null!;
        [ObservableProperty] SwitcherConfig _rawConfig = null!;
        [ObservableProperty][AlsoNotify(nameof(IsConnectButtonVisible))] SwitcherSpecs _rawSpecs = null!;
        [ObservableProperty][AlsoNotify(nameof(StatusText), nameof(ConnectionButtonText))] bool _rawIsConnected;
        [ObservableProperty][AlsoNotify(nameof(StatusText), nameof(ConnectionButtonText))] SwitcherError? _rawError;

        [ObservableProperty] ISwitcherMixBlockVM[]? _mixBlocks;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public string StatusText
		{
			get
			{
                if (RawError != null) return $"Communication Error: {RawError.Value.Exception.Message}";
				return RawIsConnected ? "Connected" : "Disconnected";
			}
		}

		public string ConnectionButtonText
		{
			get
			{
				if (RawError != null) return "Reconnect";
				return RawIsConnected ? "Disconnect" : "Connect";
			}
		}

		public bool IsConnectButtonVisible => RawSpecs.CanChangeConnection;

        public SwitcherFeatureVM(IServiceSource servSource) => _servSource = servSource;

        partial void OnRawMixBlocksChanged(IVMBinder<IVMForSwitcherMixBlock>[] value)
        {
            var newArr = new ISwitcherMixBlockVM[value.Length];
            for (int i = 0; i < newArr.Length; i++) newArr[i] = value[i].GetVM<ISwitcherMixBlockVM>(this);
            MixBlocks = newArr;
        }

        partial void OnRawConfigChanged(SwitcherConfig value) => Config = _servSource.Get<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>(RawConfig, this);

        public void UpdateConfig(SwitcherConfig config) => RawFeature.ChangeSwitcher(config);
		public void ToggleConnection()
		{
            if (RawIsConnected)
			    RawFeature.Disconnect();
            else
				RawFeature.Connect();
		}
    }
}