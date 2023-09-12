using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Presenters
{
	public interface IMainUIPresenter : IFeatureSideMenuPresenter
	{
		IMainUIVM VM { get; }
	}

	public interface IFeatureSideMenuPresenter
	{
		void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose);
		void CloseMenu();
	}

	public interface ISideMenuEmbeddableVM : INotifyPropertyChanged { }

	public class MainUIPresenter : IMainUIPresenter, IFeatureSideMenuPresenter
	{
		Action? _onClose;

		public IMainUIVM VM { get; }

		public MainUIPresenter(IServiceSource servSource)
		{
			VM = servSource.Get<IMainUIVM, IMainUIPresenter>(this);
		}

		public void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose)
		{
			_onClose = onClose;

			if (VM.MenuVM != null) CloseMenu();
			VM.MenuTitle = title;
			VM.MenuVM = vm;
		}

		public void CloseMenu()
		{
			VM.MenuTitle = "";

			if (_onClose != null)
			{
				_onClose();
				VM.MenuVM = null;
			}
		}
	}
}
