using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherFeatureVM : ILiveFeatureViewModel, INeedsInitialization<IFeature>
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
		void UpdateConfig(SwitcherConfig config);
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
		IFeature _feature = null!;

        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public void FinishConstruction(IFeature param1) => _feature = param1;

		public void UpdateConfig(SwitcherConfig config) => _feature.PerformAction((int)SwitcherFeatureActionID.ChangeConfig, config);
    }
}