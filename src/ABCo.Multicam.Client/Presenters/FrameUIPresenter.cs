using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IFrameVM : INotifyPropertyChanged
	{
		IPageVM? SelectedPage { get; }
		IFrameMenuTabVM[] TopTabs { get; }
		IFrameMenuTabVM[] MiddleTabs { get; }
		IFrameMenuTabVM[] BottomTabs { get; }
		void Select(IFrameMenuTabVM vm);
	}

	public partial class FrameVM : ViewModelBase, IFrameVM
	{
		[ObservableProperty] IPageVM? _selectedPage;
		[ObservableProperty] IFrameMenuTabVM[] _topTabs = Array.Empty<IFrameMenuTabVM>();
		[ObservableProperty] IFrameMenuTabVM[] _middleTabs = Array.Empty<IFrameMenuTabVM>();
		[ObservableProperty] IFrameMenuTabVM[] _bottomTabs = Array.Empty<IFrameMenuTabVM>();

		public FrameVM(IClientInfo info)
		{
			var switcherPage = info.ServerConnection.GetFeatures().ClientNotifier.GetOrAddClientEndpoint<ISwitcherListVM>(info);

			_topTabs = new IFrameMenuTabVM[]
			{
				new FrameMenuTabVM("Welcome", this, null)
			};

			_middleTabs = new IFrameMenuTabVM[]
			{
				new FrameMenuTabVM("Switchers / Video Devices", this, switcherPage),
				new FrameMenuTabVM("Digital Tally", this, null),
				new FrameMenuTabVM("Cut Recording", this, null)
			};

			_bottomTabs = new IFrameMenuTabVM[]
			{
				new FrameMenuTabVM("Automation", this, null),
				new FrameMenuTabVM("Sync Devices", this, null)
			};

			// Start with the very first thing selected.
			Select(TopTabs[0]);
		}

		public void Select(IFrameMenuTabVM vm)
		{
			// Deselect everything else
			foreach (IFrameMenuTabVM a in TopTabs)
				a.IsSelected = false;
			foreach (IFrameMenuTabVM b in MiddleTabs)
				b.IsSelected = false;
			foreach (IFrameMenuTabVM c in BottomTabs)
				c.IsSelected = false;

			// Select this
			vm.IsSelected = true;
			SelectedPage = vm.AssociatedPage;
		}
	}
}
