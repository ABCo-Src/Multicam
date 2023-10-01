using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.Structures;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
    public interface IProjectFeaturesPresenter : IClientNotificationTarget
	{
		IProjectFeaturesVM VM { get; }
		void OpenMobileMenu(IProjectFeaturesListItemVM vm);
		void CloseMobileMenu();
		void ToggleEdit(IProjectFeaturesListItemVM vm);
		void MoveUp(IProjectFeaturesListItemVM vm);
		void MoveDown(IProjectFeaturesListItemVM vm);
		void Delete(IProjectFeaturesListItemVM vm);
		void CreateFeature(CursorPosition pos);
	}

	public class ProjectFeaturesPresenter : IProjectFeaturesPresenter
	{
		readonly IMainUIPresenter _sideMenuPresenter;
		readonly IServerTarget _collection;
		readonly IClientInfo _info;
		readonly IUIDialogHandler _dialogHandler;

		public IProjectFeaturesVM VM { get; }
		IProjectFeaturesListItemVM? _currentlyEditing;

		public ProjectFeaturesPresenter(IServerTarget collection, IClientInfo client)
		{
			_collection = collection;
			_currentlyEditing = null;
			_dialogHandler = client.Get<IUIDialogHandler>();
			_sideMenuPresenter = client.Get<IMainUIPresenter>();
			_info = client;

			VM = client.Get<IProjectFeaturesVM, IProjectFeaturesPresenter>(this);
		}

		public void Init() => _collection.RefreshData<FeaturesList>();
        public void OnDataChange(ServerData obj)
        {
            if (obj is FeaturesList objList)
			{
				var currentFeatures = objList.Features;
				var oldItems = VM.Items;

                // Re-add each feature one-by-one, re-using the old VM if it still exists
                var newItems = new IProjectFeaturesListItemVM[currentFeatures.Count];
                for (int i = 0; i < currentFeatures.Count; i++)
                {
                    // Re-use or create a new vm
                    int vm = Array.FindIndex(oldItems, s => s.NativeItem == currentFeatures[i]);

                    if (vm == -1)
                    {
                        var innerVM = currentFeatures[i].ClientMessageDispatcher.GetOrAddClientEndpoint<IMainFeaturePresenter>(_info).VM;
                        newItems[i] = _info.Get<IProjectFeaturesListItemVM, IProjectFeaturesPresenter, IServerTarget, IFeatureVM>(this, currentFeatures[i], innerVM);
                        newItems[i].EditBtnText = "Edit";
                    }
                    else
                        newItems[i] = oldItems[vm];
                }

                VM.Items = newItems;

                // Stop editing if the vm associated with that has been removed
                EnsureCurrentlyEditingExists(currentFeatures);
            }
        }

		void EnsureCurrentlyEditingExists(IList<IServerTarget> features)
		{
			if (_currentlyEditing != null)
			{
				// Stop if this feature is still present in the new list
				for (int i = 0; i < features.Count; i++)
					if (features[i] == _currentlyEditing.NativeItem)
						return;

				// Stop editing if we weren't stopped
				_sideMenuPresenter.CloseMenu();
			}
		}

		public void ToggleEdit(IProjectFeaturesListItemVM vm)
		{
			if (_currentlyEditing == vm)
				_sideMenuPresenter.CloseMenu();
			else
			{
				_sideMenuPresenter.OpenMenu(vm, "Editing Feature", () =>
				{
					vm.EditBtnText = "Edit";
					_currentlyEditing = null;
				});

				vm.EditBtnText = "Finish";
				_currentlyEditing = vm;
			}
		}

		public void CreateFeature(CursorPosition pos)
		{
			_dialogHandler.OpenContextMenu(new ContextMenuDetails("Choose Type", HandleChoice, null, pos, new string[]
			{
				"Switcher",
				"Tally"
			}));

			void HandleChoice(string choice) => _collection.PerformAction(MainFeatureCollection.CREATE, choice switch
			{
				"Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
				_ => throw new Exception()
			});
		}

		public void OpenMobileMenu(IProjectFeaturesListItemVM vm) => VM.MobileView = vm;
		public void CloseMobileMenu() => VM.MobileView = null;
		public void MoveUp(IProjectFeaturesListItemVM vm) => _collection.PerformAction(MainFeatureCollection.MOVE_UP, vm.NativeItem);
		public void MoveDown(IProjectFeaturesListItemVM vm) => _collection.PerformAction(MainFeatureCollection.MOVE_DOWN, vm.NativeItem);
		public void Delete(IProjectFeaturesListItemVM vm) => _collection.PerformAction(MainFeatureCollection.DELETE, vm.NativeItem);
    }
}
