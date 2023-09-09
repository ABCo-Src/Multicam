using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherFeatureVM : ILiveFeatureViewModel, IParameteredService<IFeature>
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
		void UpdateConfig(SwitcherConfig config);
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
		IFeature _feature;

        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public static ISwitcherFeatureVM New(IFeature feature, IServiceSource servSource) => new SwitcherFeatureVM(feature);
		public SwitcherFeatureVM(IFeature feature) => _feature = feature;

		public void UpdateConfig(SwitcherConfig config) => _feature.PerformAction((int)SwitcherFeatureActionID.ChangeConfig, config);
    }
}