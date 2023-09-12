using ABCo.Multicam.Core;
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
	public interface IMainUIVM : IParameteredService<IMainUIPresenter>, INotifyPropertyChanged
	{
		ISideMenuEmbeddableVM? MenuVM { get; set; }
		string MenuTitle { get; set; }

		void CloseMenuButton();
	}

	public partial class MainUIVM : ViewModelBase, IMainUIVM
	{
		readonly IMainUIPresenter _presenter;

		[ObservableProperty] ISideMenuEmbeddableVM? _menuVM;
		[ObservableProperty] string _menuTitle = "";

		public MainUIVM(IMainUIPresenter presenter)
		{
			_presenter = presenter;
			//FeatureViewVM = featureViewVM;
		}

		public void CloseMenuButton() => _presenter.CloseMenu();
	}
}