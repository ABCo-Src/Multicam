using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Paging;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.General
{
	public interface IServerListVM<TItemVM> : INotifyPropertyChanged
	{
		TItemVM[] Items { get; }
		void Create();
	}

	public abstract partial class ServerListVM<TServerList, TServerListItem, TItemVM> : BoundViewModelBase<TServerList> 
		where TServerList : IServerList<TServerListItem>, INotifyPropertyChanged
		where TItemVM : IDisposable
	{
		readonly AppPages _associatedPageId;
		readonly IPopOutVM _popOutVM;

		[ObservableProperty] TItemVM[] _items = Array.Empty<TItemVM>();

		public ServerListVM(AppPages associatedPageId, Dispatched<TServerList> collection, IFrameClientInfo client) : base(collection, client)
		{
			_associatedPageId = associatedPageId;
			_popOutVM = client.Shared.PopOut;
			OnServerStateChange(null);
		}

		protected void ProcessServerListChange(Func<TServerListItem, TItemVM> createItemVM)
		{
			var items = _serverComponent.Get(c => c.Items);

			// Remove all the old VMs
			for (int i = 0; i < Items.Length; i++)
				Items[i].Dispose();

			// Add new ones
			Items = new TItemVM[items.Count];
			for (int i = 0; i < items.Count; i++)
				Items[i] = createItemVM(items[i]);

			// Stop editing
			// TODO: More selective close?
			_popOutVM.Close();
		}
	}
}