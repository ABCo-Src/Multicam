using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Client.Services;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features
{
	public interface IMainFeatureCollectionPresenter : IClientDataNotificationTarget<IMainFeatureCollection>
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

	public class MainFeatureCollectionPresenter : IMainFeatureCollectionPresenter
	{
		readonly IMainUIPresenter _sideMenuPresenter;
		readonly Dispatched<IMainFeatureCollection> _collection;
		readonly IClientInfo _info;
		readonly IUIDialogHandler _dialogHandler;

		public IProjectFeaturesVM VM { get; }
		IProjectFeaturesListItemVM? _currentlyEditing;

		public MainFeatureCollectionPresenter(Dispatched<IMainFeatureCollection> collection, IClientInfo client)
		{
			_collection = collection;
			_currentlyEditing = null;
			_dialogHandler = client.Get<IUIDialogHandler>();
			_sideMenuPresenter = client.Get<IMainUIPresenter>();
			_info = client;

			VM = client.Get<IProjectFeaturesVM, IMainFeatureCollectionPresenter>(this);
		}

		public void Init() { }
		public void OnServerStateChange(string? changedProp)
		{
			var currentFeatures = _collection.Get(c => c.Features);
			var oldItems = VM.Items;

			// Re-add each feature one-by-one, re-using the old VM if it still exists
			var newItems = new IProjectFeaturesListItemVM[currentFeatures.Count];
			for (int i = 0; i < currentFeatures.Count; i++)
			{
				// Re-use or create a new vm
				int vm = Array.FindIndex(oldItems, s => s.NativeItem == currentFeatures[i]);

				if (vm == -1)
				{
					var innerVM = currentFeatures[i].ClientNotifier.GetOrAddClientEndpoint<IFeaturePresenter>(_info).VM;
					newItems[i] = _info.Get<IProjectFeaturesListItemVM, IMainFeatureCollectionPresenter, IFeature, IFeatureVM>(this, currentFeatures[i], innerVM);
					newItems[i].EditBtnText = "Edit";
				}
				else
					newItems[i] = oldItems[vm];
			}

			VM.Items = newItems;

			// Stop editing if the vm associated with that has been removed
			EnsureCurrentlyEditingExists(currentFeatures);
		}

		void EnsureCurrentlyEditingExists(IReadOnlyList<IFeature> features)
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

			void HandleChoice(string choice) => _collection.CallDispatched(c => c.CreateFeature(choice switch
			{
				"Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
				_ => throw new Exception()
			}));
		}

		public void OpenMobileMenu(IProjectFeaturesListItemVM vm) => VM.MobileView = vm;
		public void CloseMobileMenu() => VM.MobileView = null;

		public void MoveUp(IProjectFeaturesListItemVM vm) => _collection.CallDispatched(c => c.MoveUp(vm.NativeItem));
		public void MoveDown(IProjectFeaturesListItemVM vm) => _collection.CallDispatched(c => c.MoveDown(vm.NativeItem));
		public void Delete(IProjectFeaturesListItemVM vm) => _collection.CallDispatched(c => c.Delete(vm.NativeItem));
	}
}
