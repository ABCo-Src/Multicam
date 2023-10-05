using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels
{
	public interface IMainUIVM : IClientService<IMainUIPresenter>, INotifyPropertyChanged, IAnimationHandlingVM
	{
		ISideMenuEmbeddableVM? MenuVM { get; set; }
		IProjectFeaturesVM? FeaturesVM { get; set; }
		IProjectFeaturesListItemVM? MobileFeatureViewSelectedVM { get; set; }
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

        public IProjectFeaturesVM? FeaturesVM { get; set; }

        public async void UpdateMenuVM(ISideMenuEmbeddableVM? newVal)
		{
			await WaitForAnimationHandler(nameof(MenuVM));
			_menuVM = newVal;
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(MenuVM)));
		}

		[ObservableProperty] string _menuTitle = "";

		public MainUIVM(IMainUIPresenter presenter)
		{
			_presenter = presenter;
		}

		public void CloseMenuButton() => _presenter.CloseMenu();
	}
}