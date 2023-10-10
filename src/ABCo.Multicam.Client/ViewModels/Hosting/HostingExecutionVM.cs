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
	public interface IHostingExecutionVM : INotifyPropertyChanged, IClientService<IHostingPresenter>
	{
		string LinkText { get; set; }
		string LinkHyperlink { get; set; }
		string StartStopButtonText { get; set; }
		bool CanStartStop { get; set; }
		void ToggleConnection();
	}

	public partial class HostingExecutionVM : ViewModelBase, IHostingExecutionVM
	{
		readonly IHostingPresenter _presenter;

		[ObservableProperty] string _linkText = "";
		[ObservableProperty] string _linkHyperlink = "";
		[ObservableProperty] string _startStopButtonText = "";
		[ObservableProperty] bool _canStartStop = false;

		public HostingExecutionVM(IHostingPresenter presenter) => _presenter = presenter;

		public void ToggleConnection() => _presenter.OnToggleConnection();
	}
}
