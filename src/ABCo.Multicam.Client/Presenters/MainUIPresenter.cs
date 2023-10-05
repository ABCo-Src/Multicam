using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.ViewModels;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IMainUIPresenter : IFeatureSideMenuPresenter
	{
		void Init();
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
		readonly IClientInfo _info;

		public IMainUIVM VM { get; }

        public MainUIPresenter(IClientInfo info)
        {
			_info = info;
            VM = info.Get<IMainUIVM, IMainUIPresenter>(this);
        }

        public void Init()
		{
			// Initialize the project features content
            VM.FeaturesVM = _info.ServerConnection.GetFeatures().DataStore.GetOrAddClientEndpoint<IProjectFeaturesPresenter>(_info).VM;

			// Initialize the server hosting content
        }

		public void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose)
		{
			if (_onClose != null) CloseMenu();

			_onClose = onClose;
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
