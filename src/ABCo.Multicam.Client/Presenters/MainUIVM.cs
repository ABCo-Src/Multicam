using ABCo.Multicam.Server;
using ABCo.Multicam.Client.ViewModels;
using System.ComponentModel;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Frames;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
    public interface IMainUIVM : INotifyPropertyChanged
	{
		IFrameVM Frame { get; }
		IPopOutVM PopOut { get; }
	}

	public partial class MainUIVM : ViewModelBase, IMainUIVM
	{
		[ObservableProperty] IFrameVM _frame;
		[ObservableProperty] IPopOutVM _popOut;

		public MainUIVM(IClientInfo info)
		{
			_frame = new FrameVM(info);
			_popOut = ((SharedVMs)info.Shared).PopOut; // TODO: Improve
		}
	}
}
