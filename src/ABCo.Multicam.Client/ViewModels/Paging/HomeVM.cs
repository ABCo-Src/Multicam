﻿using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Scripting.Buttons;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Client.ViewModels.Scripting.Execution;

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

            var switcherPage = new SwitcherListVM(info.Server.GetSwitcherList(), info);
            var scriptButtonPage = new ScriptButtonListVM(info.Server.GetScriptButtonList(), info);
            var consoleButtonPage = new ScriptConsoleVM(info.Server.GetScriptConsole(), info);

            _middleTabs = new IHomePageLinkVM[]
            {
                new HomePageLinkVM("Switchers / Video Devices", this, switcherPage),
                new HomePageLinkVM("Digital Tally", this, null),
                new HomePageLinkVM("Cut Recording", this, null)
            };

            _bottomTabs = new IHomePageLinkVM[]
            {
                new HomePageLinkVM("Script Buttons", this, scriptButtonPage),
                new HomePageLinkVM("Script Console", this, consoleButtonPage),
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
