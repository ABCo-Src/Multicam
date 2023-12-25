using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Paging
{
	public interface IHomeVM : IPageVM, INotifyPropertyChanged
	{
		IHomePageLinkVM[] MiddleTabs { get; }
		IHomePageLinkVM[] BottomTabs { get; }
		void Select(IHomePageLinkVM vm);
	}

	public partial class HomeVM : ViewModelBase, IHomeVM
	{
		readonly IFrameVM _frame;

		[ObservableProperty] IHomePageLinkVM[] _middleTabs = Array.Empty<IHomePageLinkVM>();
		[ObservableProperty] IHomePageLinkVM[] _bottomTabs = Array.Empty<IHomePageLinkVM>();

		public HomeVM(IFrameClientInfo info)
		{
			_frame = info.Frame;

			var switcherPage = new SwitcherListVM(new Dispatched<ISwitcherList>(info.ServerConnection.GetFeatures(), info.ServerConnection), info);

			_middleTabs = new IHomePageLinkVM[]
			{
				new HomePageLinkVM("Switchers / Video Devices", this, switcherPage),
				new HomePageLinkVM("Digital Tally", this, null),
				new HomePageLinkVM("Cut Recording", this, null)
			};

			_bottomTabs = new IHomePageLinkVM[]
			{
				new HomePageLinkVM("Automation", this, null),
				new HomePageLinkVM("Sync Devices", this, null)
			};
		}

		public AppPages Page => AppPages.Home;

		public void Select(IHomePageLinkVM vm)
		{
			_frame.SelectPage(vm.AssociatedPage);
		}
	}
}
