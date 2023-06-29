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
        readonly IUIPlatformWindowCapabilities _windowCapabilities;

        [ObservableProperty]IApplicationViewModel _application;

        public int TitleBarHeight => BorderWidth + 38;
        public int BorderWidth => _windowCapabilities.BorderRecommended ? 2 : 0;

        public MainWindowViewModel(IUIPlatformWindowCapabilities windowCapabilities, IApplicationViewModel applicationVm)
        {
            _windowCapabilities = windowCapabilities;
            Application = applicationVm;
        }
    }
}
