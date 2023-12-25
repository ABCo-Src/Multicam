using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
	public interface ISwitcherVM : INotifyPropertyChanged, IDisposable
	{
		ISwitcherMixBlocksVM MixBlocks { get; set; }
		ISwitcherConnectionVM Connection { get; set; }
		ISwitcherConfigVM Config { get; set; }
	}

	public partial class SwitcherVM : ObservableObject, ISwitcherVM
    {
		[ObservableProperty] ISwitcherMixBlocksVM _mixBlocks;
		[ObservableProperty] ISwitcherConnectionVM _connection;
		[ObservableProperty] ISwitcherConfigVM _config;

        public SwitcherVM(Dispatched<ISwitcher> feature, IFrameClientInfo info)
        {
            MixBlocks = new SwitcherMixBlocksVM(feature, info);
            Connection = new SwitcherConnectionVM(feature, info);
            Config = new SwitcherConfigVM(feature, info);
        }

		public void Dispose()
		{
			MixBlocks.Dispose();
			Connection.Dispose();
			Config.Dispose();
		}
    }
}
