using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Hosting
{
	public interface IServerHostingVM : INotifyPropertyChanged, IClientService<IHostingPresenter, IHostnameConfigVM, IHostingExecutionVM>, ISideMenuEmbeddableVM
	{
		IHostnameConfigVM HostnameVM { get; }
		IHostingExecutionVM ExecutionVM { get; }
		bool ShowConfigOptions { get; set; }
		void ToggleHostingMenu();
	}

	public partial class ServerHostingVM : ViewModelBase, IServerHostingVM
	{
		readonly IHostingPresenter _presenter;
		
		public IHostnameConfigVM HostnameVM { get; }
		public IHostingExecutionVM ExecutionVM { get; }

		[ObservableProperty] bool _showConfigOptions;

		public ServerHostingVM(IHostingPresenter presenter, IHostnameConfigVM hostnameVM, IHostingExecutionVM activeVM)
		{
			_presenter = presenter;
			HostnameVM = hostnameVM;
			ExecutionVM = activeVM;
		}

		public void ToggleHostingMenu() => _presenter.OnHostingMenuToggle();
	}
}