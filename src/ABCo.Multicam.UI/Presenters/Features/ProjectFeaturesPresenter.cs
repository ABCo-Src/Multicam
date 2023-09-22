using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.Structures;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
	public interface IProjectFeaturesPresenter : IUIPresenter, IParameteredService<IMainFeatureCollection, IScopeInfo>
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
		readonly IMainFeatureCollection _collection;
		readonly IServiceSource _servSource;
		readonly IScopeInfo _scopeInfo;
		readonly IUIDialogHandler _dialogHandler;

		public IProjectFeaturesVM VM { get; }
		IProjectFeaturesListItemVM? _currentlyEditing;

		public ProjectFeaturesPresenter(IMainFeatureCollection manager, IScopeInfo scopeInfo, IServiceSource servSource)
		{
			_collection = manager;
			_currentlyEditing = null;
			_dialogHandler = servSource.Get<IUIDialogHandler>();
			_sideMenuPresenter = servSource.Get<IMainUIPresenter>();
			_servSource = servSource;
			_scopeInfo = scopeInfo;

			VM = servSource.Get<IProjectFeaturesVM, IProjectFeaturesPresenter>(this);
		}

        public void Init() => OnDataChange(_collection.Features);
        public void OnDataChange(object obj)
        {
            if (obj is IList<IFeature> currentFeatures)
			{
				var oldItems = VM.Items;

                // Re-add each feature one-by-one, re-using the old VM if it still exists
                var newItems = new IProjectFeaturesListItemVM[_collection.Features.Count];
                for (int i = 0; i < _collection.Features.Count; i++)
                {
                    // Re-use or create a new vm
                    int vm = Array.FindIndex(oldItems, s => s.NativeItem == _collection.Features[i]);

                    if (vm == -1)
                    {
                        var innerVM = _collection.Features[i].UIPresenters.GetPresenter<IMainFeaturePresenter>(_scopeInfo).VM;
                        newItems[i] = _servSource.Get<IProjectFeaturesListItemVM, IProjectFeaturesPresenter, IFeature, IFeatureVM>(this, _collection.Features[i], innerVM);
                        newItems[i].EditBtnText = "Edit";
                    }
                    else
                        newItems[i] = oldItems[vm];
                }

                VM.Items = newItems;

                // Stop editing if the vm associated with that has been removed
                EnsureCurrentlyEditingExists();
            }
        }

		// TODO: Remove unnecessary interfaces
		// TODO: Implement proper dispose paths

		void EnsureCurrentlyEditingExists()
		{
			if (_currentlyEditing != null)
			{
				// Stop if this feature is still present in the new list
				for (int i = 0; i < _collection.Features.Count; i++)
					if (_collection.Features[i] == _currentlyEditing.NativeItem)
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

			void HandleChoice(string choice) => _collection.CreateFeature(choice switch
			{
				"Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
				_ => throw new Exception()
			});
		}

		public void OpenMobileMenu(IProjectFeaturesListItemVM vm) => VM.MobileView = vm;
		public void CloseMobileMenu() => VM.MobileView = null;
		public void MoveUp(IProjectFeaturesListItemVM vm) => _collection.MoveUp(vm.NativeItem);
		public void MoveDown(IProjectFeaturesListItemVM vm) => _collection.MoveDown(vm.NativeItem);
		public void Delete(IProjectFeaturesListItemVM vm) => _collection.Delete(vm.NativeItem);
    }
}
