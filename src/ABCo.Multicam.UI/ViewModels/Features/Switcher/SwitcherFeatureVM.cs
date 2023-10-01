using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Server.General;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherFeatureVM : IFeatureContentVM, IClientService<IServerTarget>
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
		void UpdateConfig(SwitcherConfig config);
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
		readonly IServerTarget _feature;

        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public SwitcherFeatureVM(IServerTarget feature) => _feature = feature;

		public void UpdateConfig(SwitcherConfig config) => _feature.PerformAction(SwitcherActionID.SET_CONFIG_TYPE, config);
    }
}