using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.ViewModels;
using System.ComponentModel;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Hosting;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IMainUIPresenter : IFeatureSideMenuPresenter
	{
		void Init();
		IMainUIVM? VM { get; }
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

		public IMainUIVM? VM { get; private set; }

		public MainUIPresenter(IClientInfo info) => _info = info;

		public void Init()
		{
			var featuresVM = _info.ServerConnection.GetFeatures().ClientNotifier.GetOrAddClientEndpoint<IMainFeatureCollectionPresenter>(_info).VM;
			var hostingVM = _info.ServerConnection.GetHostingManager().ClientNotifier.GetOrAddClientEndpoint<IHostingPresenter>(_info).VM;
			var frameVM = _info.Get<IFrameUIPresenter>().VM;
			VM = new MainUIVM(this, featuresVM, frameVM, hostingVM);
		}

		public void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose)
		{
			if (_onClose != null) CloseMenu();

			_onClose = onClose;
			VM!.MenuTitle = title;
			VM.MenuVM = vm;
		}

		public void CloseMenu()
		{
			VM!.MenuTitle = "";

			if (_onClose != null)
			{
				_onClose();
				VM.MenuVM = null;
			}
		}
	}
}
