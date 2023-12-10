using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using ABCo.Multicam.Client.ViewModels.Frames;
using System.ComponentModel;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switchers;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
	public interface ISwitcherListVM : IPageVM, INotifyPropertyChanged
	{
		ISwitcherListItemVM[] Items { get; }
        void CreateSwitcher();
    }

    public partial class SwitcherListVM : BoundViewModelBase<ISwitcherList>, ISwitcherListVM
    {
        readonly IPopOutVM _popOutVM;

		[ObservableProperty] ISwitcherListItemVM[] _items = Array.Empty<ISwitcherListItemVM>();

		public AppPages Page => AppPages.Switchers;

		public SwitcherListVM(Dispatched<ISwitcherList> collection, IClientInfo client) : base(collection, client)
		{
            _popOutVM = client.Shared.PopOut;
            OnServerStateChange(null);
        }

        public void Init() { }
        protected override void OnServerStateChange(string? changedProp)
        {
            var features = _serverComponent.Get(c => c.Features);

            // Remove all the old VMs
            for (int i = 0; i < Items.Length; i++)
                Items[i].Dispose();

            // Add new ones
            Items = new ISwitcherListItemVM[features.Count];
            for (int i = 0; i < features.Count; i++)
                Items[i] = new SwitcherListItemVM(_serverComponent, new Dispatched<ISwitcher>(features[i], _info.ServerConnection), _info);

            // Stop editing
            
            // TODO...
        }

		public void CreateSwitcher() => _serverComponent.CallDispatched(c => c.CreateSwitcher());
    }
}
