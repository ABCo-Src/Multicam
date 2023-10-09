using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Client.ViewModels.Hosting;

namespace ABCo.Multicam.Client.ViewModels
{
	public interface IMainUIVM : IClientService<IMainUIPresenter, IProjectFeaturesVM, IServerHostingVM>, INotifyPropertyChanged, IAnimationHandlingVM
	{
		ISideMenuEmbeddableVM? MenuVM { get; set; }
		IProjectFeaturesVM FeaturesVM { get; }
		IServerHostingVM HostingVM { get; }
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

        public IProjectFeaturesVM FeaturesVM { get; }
		public IServerHostingVM HostingVM { get; }

		public async void UpdateMenuVM(ISideMenuEmbeddableVM? newVal)
		{
			await WaitForAnimationHandler(nameof(MenuVM));
			_menuVM = newVal;
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(MenuVM)));
		}

		[ObservableProperty] string _menuTitle = "";

		public MainUIVM(IMainUIPresenter presenter, IProjectFeaturesVM featuresVM, IServerHostingVM hostingVM)
		{
			_presenter = presenter;
			FeaturesVM = featuresVM;
			HostingVM = hostingVM;
		}

		public void CloseMenuButton() => _presenter.CloseMenu();
	}
}