using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers.Config.Virtual
{
    public interface ISwitcherVirtualConfigVM : ISwitcherSpecificConfigVM
    {
        string SelectedMixBlockCount { get; set; }
        int[] MixBlockCountOptions { get; }
        ISwitcherVirtualConfigMixBlockVM[] MixBlockVMs { get; set; }
        void OnUIChange();
    }

    public partial class SwitcherVirtualConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherVirtualConfigVM
    {
        public int[] MixBlockCountOptions => new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [ObservableProperty] string _selectedMixBlockCount = "1";
        [ObservableProperty] ISwitcherVirtualConfigMixBlockVM[] _mixBlockVMs = Array.Empty<ISwitcherVirtualConfigMixBlockVM>();

        public SwitcherVirtualConfigVM(Dispatched<ISwitcher> feature, IFrameClientInfo info) : base(feature, info) => OnServerStateChange(null);

        public void OnUIChange()
        {
            var chosenCount = int.Parse(SelectedMixBlockCount);
            var newConfigMBs = new int[chosenCount];

            // Start with 1
            Array.Fill(newConfigMBs, 1);

            // Fill in from all the currently existing VMs
            int end = Math.Min(MixBlockVMs.Length, chosenCount);
            for (int i = 0; i < end; i++)
                newConfigMBs[i] = int.Parse(MixBlockVMs[i].InputCount);

            _serverComponent.CallDispatched(f => f.ChangeConfig(new VirtualSwitcherConfig(newConfigMBs)));
        }

        protected override void OnServerStateChange(string? changedProp)
        {
            var config = _serverComponent.Get(s => s.Config);
            if (config is VirtualSwitcherConfig dummyConfig)
            {
                // Set the selected count
                SelectedMixBlockCount = dummyConfig.MixBlocks.Length.ToString();

                // Add the mix-blocks
                var newMixBlocks = new ISwitcherVirtualConfigMixBlockVM[dummyConfig.MixBlocks.Length];
                for (int i = 0; i < dummyConfig.MixBlocks.Length; i++)
                    newMixBlocks[i] = new SwitcherVirtualConfigMixBlockVM(this, dummyConfig.MixBlocks[i], i);

                MixBlockVMs = newMixBlocks;
            }
        }
    }
}
