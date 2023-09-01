using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
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
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsConnectButtonVisible))] SwitcherSpecs _rawSpecs = null!;
        [ObservableProperty] SwitcherConfig _rawConfig = null!;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(StatusText))] bool _rawIsConnected;

        [ObservableProperty] ISwitcherMixBlockVM[]? _mixBlocks;
        [ObservableProperty] ISwitcherConfigVM? _config;

        public string StatusText => RawIsConnected ? "Connected: No Errors" : "Disconnected";
		public string ConnectionButtonText => RawIsConnected ? "Disconnect" : "Connect";
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