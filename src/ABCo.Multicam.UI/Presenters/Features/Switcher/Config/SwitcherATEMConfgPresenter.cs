using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Config.ATEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher.Config
{
	public interface ISwitcherATEMConfgPresenter : ISwitcherSpecificConfigPresenter, IParameteredService<IFeature>
	{
		void OnUIChange();
	}

	public class SwitcherATEMConfgPresenter : ISwitcherATEMConfgPresenter
	{
		readonly IFeature _feature;
		readonly ISwitcherATEMConfigVM _vm;

		public ISwitcherSpecificConfigVM VM => _vm;

		public SwitcherATEMConfgPresenter(IFeature feature, IServiceSource servSource)
		{
			_vm = servSource.Get<ISwitcherATEMConfigVM, ISwitcherATEMConfgPresenter>(this);
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
	}
}
