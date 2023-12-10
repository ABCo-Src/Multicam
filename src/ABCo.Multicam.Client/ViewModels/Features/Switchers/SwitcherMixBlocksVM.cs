using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
	public interface ISwitcherMixBlocksVM : INotifyPropertyChanged, IDisposable
    {
		ISwitcherMixBlockVM[] MixBlocks { get; }
	}

    public partial class SwitcherMixBlocksVM : BoundViewModelBase<ISwitcher>, ISwitcherMixBlocksVM
    {
		[ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();

        SwitcherSpecs? _lastSeenSpecs = null;

		public SwitcherMixBlocksVM(Dispatched<ISwitcher> feature, IClientInfo info) : base(feature, info) => OnServerStateChange(null);

		protected override void OnServerStateChange(string? changedProp)
		{
			var specs = _serverComponent.Get(m => m.SpecsInfo);

			// If the specs have changed from what we remember, recreate everything
			if (_lastSeenSpecs != specs.Specs)
			{
				_lastSeenSpecs = specs.Specs;

				var newMixBlocks = new ISwitcherMixBlockVM[_lastSeenSpecs.MixBlocks.Count];
                for (int i = 0; i < newMixBlocks.Length; i++)
                    newMixBlocks[i] = new SwitcherMixBlockVM(_serverComponent, specs.Specs.MixBlocks[i], i);

				MixBlocks = newMixBlocks;
			}

			// Update the state
			for (int i = 0; i < MixBlocks.Length; i++)
				MixBlocks[i].UpdateState(specs.State[i]);
		}

        public void Cut(int mixBlock) => _serverComponent.CallDispatched(f => f.Cut(mixBlock));
	}
}
