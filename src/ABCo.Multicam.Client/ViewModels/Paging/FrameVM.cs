using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Client.ViewModels.Paging;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
    public interface IFrameVM : INotifyPropertyChanged
    {
        string? FrameTitle { get; }
        IPageVM? CurrentPage { get; }
        bool CanGoBack { get; }
        void NavigateBack();
        void SelectPage(IPageVM? selectedPage);
    }

    public partial class FrameVM : ViewModelBase, IFrameVM
    {
        Stack<IPageVM?> _pageHistory = new();

        [ObservableProperty] IPageVM? _currentPage;
        [ObservableProperty] string? _frameTitle;
        [ObservableProperty] bool _canGoBack;

        public FrameVM(IClientInfo info)
        {
            _currentPage = new HomeVM(info.NewFrameClientInfo(this));
            _pageHistory.Push(_currentPage);
            RefreshGeneralInfo();
        }

        public void SelectPage(IPageVM? selectedPage)
        {
            _pageHistory.Push(CurrentPage);
            CurrentPage = selectedPage;
            RefreshGeneralInfo();
        }

        public void NavigateBack()
        {
            if (_pageHistory.Count == 1) return;
            CurrentPage = _pageHistory.Pop();
            RefreshGeneralInfo();
        }

        void RefreshGeneralInfo()
        {
            FrameTitle = CurrentPage == null ? "" : CurrentPage.Page switch
            {
                AppPages.Home => "Home",
                AppPages.Switchers => "Switchers",
                AppPages.ScriptButtons => "Script Buttons",
                _ => ""
            };

            CanGoBack = _pageHistory.Count > 1;
        }
    }
}
