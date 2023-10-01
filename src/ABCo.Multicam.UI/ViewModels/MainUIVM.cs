using ABCo.Multicam.Server;
using ABCo.Multicam.UI.Presenters;
using ABCo.Multicam.UI.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels
{
	public interface IMainUIVM : IClientService<IMainUIPresenter>, INotifyPropertyChanged, IAnimationHandlingVM
	{
		ISideMenuEmbeddableVM? MenuVM { get; set; }
		IProjectFeaturesVM? ContentVM { get; set; }
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

        public IProjectFeaturesVM? ContentVM { get; set; }

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