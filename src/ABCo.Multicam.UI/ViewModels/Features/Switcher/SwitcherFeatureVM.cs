using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherFeatureVM : IFeatureContentVM, IParameteredService<IFeature>
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
		void UpdateConfig(SwitcherConfig config);
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
		readonly IFeature _feature;

        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public SwitcherFeatureVM(IFeature feature) => _feature = feature;

		public void UpdateConfig(SwitcherConfig config) => _feature.InteractionHandler.PerformAction(SwitcherActionID.SET_CONFIG_TYPE, config);
    }
}