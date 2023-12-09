using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers.Config
{
    public interface ISwitcherDummyConfigPresenter : ISwitcherSpecificConfigPresenter, IClientService<Dispatched<ISwitcherFeature>>
    {
        void OnUIChange();
    }

    public class SwitcherDummyConfigPresenter : ISwitcherDummyConfigPresenter
    {
        readonly IClientInfo _servSource;
        readonly Dispatched<ISwitcherFeature> _feature;
        readonly ISwitcherDummyConfigVM _vm;

        public ISwitcherSpecificConfigVM VM => _vm;

        public SwitcherDummyConfigPresenter(Dispatched<ISwitcherFeature> feature, IClientInfo servSource)
        {
            _servSource = servSource;
            _feature = feature;
            _vm = servSource.Get<ISwitcherDummyConfigVM, ISwitcherDummyConfigPresenter>(this);
        }

        public void Refresh(SwitcherConfig config)
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

        public void OnCompatibility(SwitcherPlatformCompatibilityValue compatibility) { }

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

            _feature.CallDispatched(f => f.ChangeConfig(new DummySwitcherConfig(newConfigMBs)));
        }
    }
}
