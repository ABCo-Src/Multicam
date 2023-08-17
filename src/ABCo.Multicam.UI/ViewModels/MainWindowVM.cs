using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels
{
    public partial class MainWindowVM : ViewModelBase
    {
        readonly IUIWindow _window;

        [ObservableProperty] IApplicationVM _application;

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

        public MainWindowVM(IServiceSource source, IUIWindow window)
        {
            _window = window;
            _application = source.Get<IApplicationVM>();
        }
    }
}
