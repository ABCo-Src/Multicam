using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers.Config.Dummy
{
	public interface ISwitcherDummyConfigVM : ISwitcherSpecificConfigVM
    {
		string SelectedMixBlockCount { get; set; }
		int[] MixBlockCountOptions { get; }
		ISwitcherDummyConfigMixBlockVM[] MixBlockVMs { get; set; }
		void OnUIChange();
    }

	public partial class SwitcherDummyConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherDummyConfigVM
    {
		public int[] MixBlockCountOptions => new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		[ObservableProperty] string _selectedMixBlockCount = "1";
		[ObservableProperty] ISwitcherDummyConfigMixBlockVM[] _mixBlockVMs = Array.Empty<ISwitcherDummyConfigMixBlockVM>();

		public SwitcherDummyConfigVM(Dispatched<ISwitcher> feature, IClientInfo info) : base(feature, info) { }

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

            _serverComponent.CallDispatched(f => f.ChangeConfig(new DummySwitcherConfig(newConfigMBs)));
        }

		protected override void OnServerStateChange(string? changedProp)
		{
			var config = _serverComponent.Get(s => s.Config);
			var dummyConfig = (DummySwitcherConfig)config;

			// Set the selected count
			SelectedMixBlockCount = dummyConfig.MixBlocks.Length.ToString();

			// Add the mix-blocks
			var newMixBlocks = new ISwitcherDummyConfigMixBlockVM[dummyConfig.MixBlocks.Length];
			for (int i = 0; i < dummyConfig.MixBlocks.Length; i++)
				newMixBlocks[i] = new SwitcherDummyConfigMixBlockVM(this, dummyConfig.MixBlocks[i], i);

			MixBlockVMs = newMixBlocks;
		}
	}
}
