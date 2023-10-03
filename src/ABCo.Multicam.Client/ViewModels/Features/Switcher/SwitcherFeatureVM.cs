using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
    public interface ISwitcherFeatureVM : IFeatureContentVM, IClientService<IServerTarget>
    {
		ISwitcherMixBlockVM[] MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM? Config { get; set; }
	}

    public partial class SwitcherFeatureVM : ViewModelBase, ISwitcherFeatureVM
    {
		readonly IServerTarget _feature;

        [ObservableProperty] ISwitcherMixBlockVM[] _mixBlocks = Array.Empty<ISwitcherMixBlockVM>();
        [ObservableProperty] ISwitcherConnectionVM _connection = null!;
        [ObservableProperty] ISwitcherConfigVM? _config;

		public SwitcherFeatureVM(IServerTarget feature) => _feature = feature;
    }
}