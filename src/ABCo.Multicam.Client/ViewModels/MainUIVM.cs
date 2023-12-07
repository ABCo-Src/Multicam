using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Client.ViewModels.Frames;

namespace ABCo.Multicam.Client.ViewModels
{
	public interface IMainUIVM : INotifyPropertyChanged, IAnimationHandlingVM
	{
		ISideMenuEmbeddableVM? MenuVM { get; set; }
		IProjectFeaturesVM FeaturesVM { get; }
		IServerHostingVM HostingVM { get; }
		IFrameVM Frame { get; }
        string MenuTitle { get; set; }

		void CloseMenuButton();
	}

	public partial class MainUIVM : ViewModelBase, IMainUIVM
	{
		readonly IMainUIPresenter _presenter;

		ISideMenuEmbeddableVM? _menuVM;
		public ISideMenuEmbeddableVM? MenuVM
		{
			get => _menuVM;
			set => UpdateMenuVM(value);
		}

		public IFrameVM Frame { get; }
		public IProjectFeaturesVM FeaturesVM { get; }
		public IServerHostingVM HostingVM { get; }

		public async void UpdateMenuVM(ISideMenuEmbeddableVM? newVal)
		{
			await WaitForAnimationHandler(nameof(MenuVM));
			_menuVM = newVal;
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(MenuVM)));
		}

		[ObservableProperty] string _menuTitle = "";

		public MainUIVM(IMainUIPresenter presenter, IProjectFeaturesVM featuresVM, IFrameVM frame, IServerHostingVM hostingVM)
		{
			_presenter = presenter;
			FeaturesVM = featuresVM;
			Frame = frame;
			HostingVM = hostingVM;
		}

		public void CloseMenuButton() => _presenter.CloseMenu();
	}
}