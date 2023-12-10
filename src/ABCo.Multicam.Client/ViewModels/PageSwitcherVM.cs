using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IPageSwitcherVM : INotifyPropertyChanged
	{
		IPageVM? SelectedPage { get; }
		IPageSwitcherMenuTabVM[] TopTabs { get; }
		IPageSwitcherMenuTabVM[] MiddleTabs { get; }
		IPageSwitcherMenuTabVM[] BottomTabs { get; }
		void Select(IPageSwitcherMenuTabVM vm);
	}

	public partial class PageSwitcherVM : ViewModelBase, IPageSwitcherVM
	{
		[ObservableProperty] IPageVM? _selectedPage;
		[ObservableProperty] IPageSwitcherMenuTabVM[] _topTabs = Array.Empty<IPageSwitcherMenuTabVM>();
		[ObservableProperty] IPageSwitcherMenuTabVM[] _middleTabs = Array.Empty<IPageSwitcherMenuTabVM>();
		[ObservableProperty] IPageSwitcherMenuTabVM[] _bottomTabs = Array.Empty<IPageSwitcherMenuTabVM>();

		public PageSwitcherVM(IClientInfo info)
		{
			var switcherPage = new SwitcherListVM(new Dispatched<ISwitcherList>(info.ServerConnection.GetFeatures(), info.ServerConnection), info);

			_topTabs = new IPageSwitcherMenuTabVM[]
			{
				new PageSwitcherMenuTabVM("Welcome", this, null)
			};

			_middleTabs = new IPageSwitcherMenuTabVM[]
			{
				new PageSwitcherMenuTabVM("Switchers / Video Devices", this, switcherPage),
				new PageSwitcherMenuTabVM("Digital Tally", this, null),
				new PageSwitcherMenuTabVM("Cut Recording", this, null)
			};

			_bottomTabs = new IPageSwitcherMenuTabVM[]
			{
				new PageSwitcherMenuTabVM("Automation", this, null),
				new PageSwitcherMenuTabVM("Sync Devices", this, null)
			};

			// Start with the very first thing selected.
			Select(TopTabs[0]);
		}

		public void Select(IPageSwitcherMenuTabVM vm)
		{
			// Deselect everything else
			foreach (IPageSwitcherMenuTabVM a in TopTabs)
				a.IsSelected = false;
			foreach (IPageSwitcherMenuTabVM b in MiddleTabs)
				b.IsSelected = false;
			foreach (IPageSwitcherMenuTabVM c in BottomTabs)
				c.IsSelected = false;

			// Select this
			vm.IsSelected = true;
			SelectedPage = vm.AssociatedPage;
		}
	}
}
