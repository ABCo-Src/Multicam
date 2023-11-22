using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Config.ATEM;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher.Config
{
	public interface ISwitcherATEMConfigPresenter : ISwitcherSpecificConfigPresenter, IClientService<Dispatched<ISwitcherFeature>>
	{
		void OnUIChange();
	}

	public class SwitcherATEMConfigPresenter : ISwitcherATEMConfigPresenter
	{
		readonly Dispatched<ISwitcherFeature> _feature;
		readonly ISwitcherATEMConfigVM _vm;

		public ISwitcherSpecificConfigVM VM => _vm;

		public SwitcherATEMConfigPresenter(Dispatched<ISwitcherFeature> feature, IClientInfo servSource)
		{
			_vm = servSource.Get<ISwitcherATEMConfigVM, ISwitcherATEMConfigPresenter>(this);
			_feature = feature;
		}

		public void Refresh(SwitcherConfig config)
		{
			var atemConfig = (ATEMSwitcherConfig)config;

			_vm.SelectedConnectionType = atemConfig.IP == null ? "USB" : "IP";			
			if (atemConfig.IP != null)
				_vm.IpAddress = atemConfig.IP;
		}

		public void OnUIChange()
		{
			var newATEMConfig = new ATEMSwitcherConfig(_vm.SelectedConnectionType == "USB" ? null : _vm.IpAddress);
			_feature.CallDispatched(f => f.ChangeConfig(newATEMConfig));
		}

		public void OnCompatibility(SwitcherPlatformCompatibilityValue compatibility) => _vm.CompatibilityMessage = compatibility;
	}
}
