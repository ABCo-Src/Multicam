using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Frames
{
    public interface IFrameVM : INotifyPropertyChanged
	{
        IPageVM? SelectedPage { get; set; }
        IFrameMenuTabVM[] TopTabs { get; }
		IFrameMenuTabVM[] MiddleTabs { get; }
		IFrameMenuTabVM[] BottomTabs { get; }
	}

    public partial class FrameVM : ViewModelBase, IFrameVM
    {
        [ObservableProperty] IPageVM? _selectedPage;
        [ObservableProperty] IFrameMenuTabVM[] _topTabs = Array.Empty<IFrameMenuTabVM>();
        [ObservableProperty] IFrameMenuTabVM[] _middleTabs = Array.Empty<IFrameMenuTabVM>();
        [ObservableProperty] IFrameMenuTabVM[] _bottomTabs = Array.Empty<IFrameMenuTabVM>();

        public FrameVM(IPageVM? selectedPage, IFrameMenuTabVM[] topTabs, IFrameMenuTabVM[] middleTabs, IFrameMenuTabVM[] bottomTabs)
		{
			SelectedPage = selectedPage;
			TopTabs = topTabs;
			MiddleTabs = middleTabs;
			BottomTabs = bottomTabs;
		}
	}
}
