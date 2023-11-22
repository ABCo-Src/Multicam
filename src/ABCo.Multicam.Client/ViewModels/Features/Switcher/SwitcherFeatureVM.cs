using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherFeatureVM : IFeatureContentVM
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public SwitcherFeatureVM() { }
    }
}