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
		IClientInfo _info;

		public IMainUIVM VM { get; }

        public MainUIPresenter(IClientInfo info)
        {
			_info = info;
            VM = info.Get<IMainUIVM, IMainUIPresenter>(this);
        }

        public void Init()
		{
            var mainFeaturesCollection = _info.ServerConnection.GetFeatures();
			VM.ContentVM = mainFeaturesCollection.ClientMessageDispatcher.GetOrAddClientEndpoint<IProjectFeaturesPresenter>(_info).VM;
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
