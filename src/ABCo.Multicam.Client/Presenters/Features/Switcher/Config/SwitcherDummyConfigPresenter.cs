using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher.Config
{
	public interface ISwitcherDummyConfigPresenter : ISwitcherSpecificConfigPresenter, IClientService<IServerTarget>
	{
		void OnUIChange();
	}

	public class SwitcherDummyConfigPresenter : ISwitcherDummyConfigPresenter
	{
		readonly IClientInfo _servSource;
		readonly IServerTarget _feature;
		readonly ISwitcherDummyConfigVM _vm;

		public ISwitcherSpecificConfigVM VM => _vm;
		
		public SwitcherDummyConfigPresenter(IServerTarget feature, IClientInfo servSource)
		{
			_servSource = servSource;
			_feature = feature;
			_vm = servSource.Get<ISwitcherDummyConfigVM, ISwitcherDummyConfigPresenter>(this);
		}

		public void OnConfig(SwitcherConfig config)
		{
			var dummyConfig = (DummySwitcherConfig)config;

			// Set the selected count
			_vm.SelectedMixBlockCount = dummyConfig.MixBlocks.Length.ToString();

			// Add the mix-blocks
			var newMixBlocks = new ISwitcherDummyConfigMixBlockVM[dummyConfig.MixBlocks.Length];
			for (int i = 0; i < dummyConfig.MixBlocks.Length; i++)
			{
				newMixBlocks[i] = _servSource.Get<ISwitcherDummyConfigMixBlockVM, ISwitcherDummyConfigPresenter>(this);
				newMixBlocks[i].InputCount = dummyConfig.MixBlocks[i].ToString();
				newMixBlocks[i].InputIndex = i + 1;
			}
			_vm.MixBlockVMs = newMixBlocks;
		}

		public void OnCompatibility(SwitcherCompatibility compatibility) { }

		public void OnUIChange()
		{
			var chosenCount = int.Parse(_vm.SelectedMixBlockCount);
			var newConfigMBs = new int[chosenCount];

			// Start with 1
			Array.Fill(newConfigMBs, 1);

			// Fill in from all the currently existing VMs
			int end = Math.Min(_vm.MixBlockVMs.Length, chosenCount);
			for (int i = 0; i < end; i++)
				newConfigMBs[i] = int.Parse(_vm.MixBlockVMs[i].InputCount);

			_feature.PerformAction(SwitcherLiveFeature.SET_CONFIG, new DummySwitcherConfig(newConfigMBs));
		}
	}
}
