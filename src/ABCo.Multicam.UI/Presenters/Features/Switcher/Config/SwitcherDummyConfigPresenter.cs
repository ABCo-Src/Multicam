using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher.Config
{
	public interface ISwitcherDummyConfigPresenter : ISwitcherSpecificConfigPresenter, IParameteredService<IFeature>
	{
		void OnChange();
	}

	public class SwitcherDummyConfigPresenter : ISwitcherDummyConfigPresenter
	{
		readonly IServiceSource _servSource;
		readonly IFeature _feature;
		IDummySwitcherConfigVM _vm;

		public ISwitcherSpecificConfigVM VM => _vm;
		
		public SwitcherDummyConfigPresenter(IFeature feature, IServiceSource servSource)
		{
			_servSource = servSource;
			_feature = feature;
			_vm = servSource.Get<IDummySwitcherConfigVM, ISwitcherDummyConfigPresenter>(this);
		}

		public void OnConfig(SwitcherConfig config)
		{
			var dummyConfig = (DummySwitcherConfig)config;

			// Set the selected count
			_vm.SelectedMixBlockCount = dummyConfig.MixBlocks.Length.ToString();

			// Add the mix-blocks
			var newMixBlocks = new IDummySwitcherConfigMixBlockVM[dummyConfig.MixBlocks.Length];
			for (int i = 0; i < dummyConfig.MixBlocks.Length; i++)
			{
				newMixBlocks[i] = _servSource.Get<IDummySwitcherConfigMixBlockVM, ISwitcherDummyConfigPresenter>(this);
				newMixBlocks[i].InputCount = dummyConfig.MixBlocks[i].ToString();
				newMixBlocks[i].InputIndex = i + 1;
			}
			_vm.MixBlockVMs = newMixBlocks;
		}

		public void OnChange()
		{
			var chosenCount = int.Parse(_vm.SelectedMixBlockCount);
			var newConfigMBs = new int[chosenCount];

			// Start with 1
			Array.Fill(newConfigMBs, 1);

			// Fill in from all the currently existing VMs
			int end = Math.Min(_vm.MixBlockVMs.Length, chosenCount);
			for (int i = 0; i < end; i++)
				newConfigMBs[i] = int.Parse(_vm.MixBlockVMs[i].InputCount);

			_feature.PerformAction(SwitcherActionID.SET_CONFIG, new DummySwitcherConfig(newConfigMBs));
		}
	}
}
