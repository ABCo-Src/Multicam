using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Client.Services;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using ABCo.Multicam.Client.ViewModels.Frames;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
    public interface ISwitcherListVM : IClientDataNotificationTarget<IMainFeatureCollection>, IPageVM, INotifyPropertyChanged
	{
		ISwitcherListItemVM[] Items { get; }
		void MoveUp(ISwitcherListItemVM vm);
        void MoveDown(ISwitcherListItemVM vm);
        void Delete(ISwitcherListItemVM vm);
        void CreateFeature();
    }

    public partial class SwitcherListVM : ViewModelBase, ISwitcherListVM
    {
        readonly Dispatched<IMainFeatureCollection> _collection;
        readonly IClientInfo _info;
        readonly IPopOutVM _popOutVM;

		[ObservableProperty] ISwitcherListItemVM[] _items = Array.Empty<ISwitcherListItemVM>();

		public AppPages Page => AppPages.Switchers;

		public SwitcherListVM(Dispatched<IMainFeatureCollection> collection, IClientInfo client)
        {
            _collection = collection;
            _popOutVM = ((SharedVMs)client.Shared).PopOut;
            _info = client;           

        }

        public void Init() { }
        public void OnServerStateChange(string? changedProp)
        {
            var currentFeatures = _collection.Get(c => c.Features);
            var oldItems = Items;

            // Re-add each feature one-by-one, re-using the old VM if it still exists
            var newItems = new ISwitcherListItemVM[currentFeatures.Count];
            for (int i = 0; i < currentFeatures.Count; i++)
            {
                // Re-use or create a new vm
                int vm = Array.FindIndex(oldItems, s => s.NativeItem == currentFeatures[i]);

                if (vm == -1)
                {
                    var innerVM = currentFeatures[i].ClientNotifier.GetOrAddClientEndpoint<IFeaturePresenter>(_info).VM;
                    newItems[i] = _info.Get<ISwitcherListItemVM, ISwitcherListVM, IFeature, IFeatureVM>(this, currentFeatures[i], innerVM);
                    newItems[i].EditBtnText = "Edit";
                }
                else
                    newItems[i] = oldItems[vm];
            }

            Items = newItems;

            // Stop editing
            
            // TODO...
        }

		public void CreateFeature() => _collection.CallDispatched(c => c.CreateSwitcher());

		public void MoveUp(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.MoveUp(vm.NativeItem));
        public void MoveDown(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.MoveDown(vm.NativeItem));
        public void Delete(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.Delete(vm.NativeItem));
    }
}
