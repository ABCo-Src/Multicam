using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IMainUIVM : INotifyPropertyChanged
	{
		IFrameVM Frame1 { get; }
		IFrameVM Frame2 { get; }
		IPopOutVM PopOut { get; }
	}

	public partial class MainUIVM : ViewModelBase, IMainUIVM
	{
		[ObservableProperty] IFrameVM _frame1;
		[ObservableProperty] IFrameVM _frame2;
		[ObservableProperty] IPopOutVM _popOut;

		public MainUIVM(IClientInfo info)
		{
			_frame1 = new FrameVM(info);
			_frame2 = new FrameVM(info);
			_popOut = info.Shared.PopOut;
		}
	}
}
