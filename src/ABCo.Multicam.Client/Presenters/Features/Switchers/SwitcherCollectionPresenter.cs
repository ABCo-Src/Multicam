using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Client.Services;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
    public interface ISwitcherCollectionPresenter : IClientDataNotificationTarget<IMainFeatureCollection>
    {
        ISwitcherListVM VM { get; }
        void MoveUp(ISwitcherListItemVM vm);
        void MoveDown(ISwitcherListItemVM vm);
        void Delete(ISwitcherListItemVM vm);
        void CreateFeature(CursorPosition pos);
    }

    public class SwitcherCollectionPresenter : ISwitcherCollectionPresenter
    {
        readonly Dispatched<IMainFeatureCollection> _collection;
        readonly IClientInfo _info;
        readonly IPopOutVM _popOutVM;

        public ISwitcherListVM VM { get; }

        public SwitcherCollectionPresenter(Dispatched<IMainFeatureCollection> collection, IClientInfo client)
        {
            _collection = collection;
            _popOutVM = ((SharedVMs)client.Shared).PopOut;
            _info = client;

            VM = client.Get<ISwitcherListVM, ISwitcherCollectionPresenter>(this);
        }

        public void Init() { }
        public void OnServerStateChange(string? changedProp)
        {
            var currentFeatures = _collection.Get(c => c.Features);
            var oldItems = VM.Items;

            // Re-add each feature one-by-one, re-using the old VM if it still exists
            var newItems = new ISwitcherListItemVM[currentFeatures.Count];
            for (int i = 0; i < currentFeatures.Count; i++)
            {
                // Re-use or create a new vm
                int vm = Array.FindIndex(oldItems, s => s.NativeItem == currentFeatures[i]);

                if (vm == -1)
                {
                    var innerVM = currentFeatures[i].ClientNotifier.GetOrAddClientEndpoint<IFeaturePresenter>(_info).VM;
                    newItems[i] = _info.Get<ISwitcherListItemVM, ISwitcherCollectionPresenter, IFeature, IFeatureVM>(this, currentFeatures[i], innerVM);
                    newItems[i].EditBtnText = "Edit";
                }
                else
                    newItems[i] = oldItems[vm];
            }

            VM.Items = newItems;

            // Stop editing
            // TODO...
        }

        public void CreateFeature(CursorPosition pos)
        {
			_popOutVM.OpenContext(new ContextMenuDetails("Choose Type", HandleChoice, null, pos, new string[]
            {
                "Switcher",
                "Tally"
            }));

            void HandleChoice(string choice) => _collection.CallDispatched(c => c.CreateSwitcher());
        }

        public void MoveUp(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.MoveUp(vm.NativeItem));
        public void MoveDown(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.MoveDown(vm.NativeItem));
        public void Delete(ISwitcherListItemVM vm) => _collection.CallDispatched(c => c.Delete(vm.NativeItem));
    }
}
