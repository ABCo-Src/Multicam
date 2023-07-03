using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        readonly IUIWindow _window;

        [ObservableProperty] ApplicationViewModel _application;

        public int TitleBarHeight => BorderWidth + 38;
        public int BorderWidth => _window.BorderRecommended ? 4 : 0;

        public bool ShowClose => _window.CloseBtnRecommended;
        public bool ShowMaximize => _window.CanMaximize;
        public bool ShowMinimize => _window.CanMinimize;

        // Kept up-to-date by the window itself
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BorderWidth))] bool _isMaximized;

        public void Close() => _window.CloseMainWindow();
        public void RequestMaximizeToggle() => _window.RequestMainWindowMaximizeToggle();
        public void RequestMinimize() => _window.RequestMainWindowMinimize();

        public MainWindowViewModel(IServiceSource source, IUIWindow window)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            _window = window;
            _application = new ApplicationViewModel(source);
        }
    }
}
