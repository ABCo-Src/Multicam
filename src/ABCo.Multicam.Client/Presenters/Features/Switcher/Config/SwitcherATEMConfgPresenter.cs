using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Live.Types.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Config.ATEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher.Config
{
    public interface ISwitcherATEMConfigPresenter : ISwitcherSpecificConfigPresenter, IClientService<IServerTarget>
	{
		void OnUIChange();
	}

	public class SwitcherATEMConfgPresenter : ISwitcherATEMConfigPresenter
	{
		readonly IServerTarget _feature;
		readonly ISwitcherATEMConfigVM _vm;

		public ISwitcherSpecificConfigVM VM => _vm;

		public SwitcherATEMConfgPresenter(IServerTarget feature, IClientInfo servSource)
		{
			_vm = servSource.Get<ISwitcherATEMConfigVM, ISwitcherATEMConfigPresenter>(this);
			_feature = feature;
		}

		public void OnConfig(SwitcherConfig config)
		{
			var atemConfig = (ATEMSwitcherConfig)config;

			_vm.SelectedConnectionType = atemConfig.IP == null ? "USB" : "IP";			
			if (atemConfig.IP != null)
				_vm.IpAddress = atemConfig.IP;
		}

		public void OnUIChange()
		{
			var newATEMConfig = new ATEMSwitcherConfig(_vm.SelectedConnectionType == "USB" ? null : _vm.IpAddress);
			_feature.PerformAction(SwitcherActionID.SET_CONFIG, newATEMConfig);
		}

		public void OnCompatibility(SwitcherCompatibility compatibility) => _vm.CompatibilityMessage = compatibility.Value;
	}
}
