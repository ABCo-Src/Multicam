using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Paging;
using ABCo.Multicam.Client.ViewModels.General;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
    public interface ISwitcherListVM : IServerListVM<ISwitcherListItemVM>, IPageVM, INotifyPropertyChanged
	{
        void CreateSwitcher();
    }

    public partial class SwitcherListVM : ServerListVM<ISwitcherList, ISwitcher, ISwitcherListItemVM>, ISwitcherListVM
    {
		public AppPages Page => AppPages.Switchers;

		public SwitcherListVM(Dispatched<ISwitcherList> collection, IFrameClientInfo client) : base(AppPages.Switchers, collection, client) { }

        protected override void OnServerStateChange(string? changedProp) =>
            ProcessServerListChange(s => new SwitcherListItemVM(_serverComponent, _info.CreateServerDispatcher(s), _info));

		public void CreateSwitcher() => _serverComponent.CallDispatched(c => c.CreateSwitcher());
    }
}
