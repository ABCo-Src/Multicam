using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.Structures;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
	public interface IProjectFeaturesPresenterForVM : IProjectFeaturesPresenter, IParameteredService<IMainFeatureCollection>
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

	public class ProjectFeaturesPresenter : IProjectFeaturesPresenterForVM
	{
		readonly IMainUIPresenter _sideMenuPresenter;
		readonly IMainFeatureCollection _manager;
		readonly IServiceSource _servSource;
		readonly IUIDialogHandler _dialogHandler;

		public IProjectFeaturesVM VM { get; }
		IProjectFeaturesListItemVM? _currentlyEditing;

		public ProjectFeaturesPresenter(IMainFeatureCollection manager, IServiceSource servSource)
		{
			_manager = manager;
			_currentlyEditing = null;
			_dialogHandler = servSource.Get<IUIDialogHandler>();
			_sideMenuPresenter = servSource.Get<IMainUIPresenter>();
			_servSource = servSource;
			VM = servSource.Get<IProjectFeaturesVM, IProjectFeaturesPresenterForVM>(this);
		}

		public void OnItemsChange()
		{
			var oldItems = VM.Items;

			// Re-add each feature one-by-one, re-using the old VM if it still exists
			var newItems = new IProjectFeaturesListItemVM[_manager.Features.Count];
			for (int i = 0; i < _manager.Features.Count; i++)
			{
				// Re-use or create a new vm
				int vm = Array.FindIndex(oldItems, s => s.NativeItem == _manager.Features[i]);

				if (vm == -1)
				{
					var innerVM = ((IFeaturePresenterForVM)_manager.Features[i].UIPresenter).VM;
					newItems[i] = _servSource.Get<IProjectFeaturesListItemVM, IProjectFeaturesPresenterForVM, IFeature, IFeatureVM>(this, _manager.Features[i], innerVM);
					newItems[i].EditBtnText = "Edit";
				}
				else
					newItems[i] = oldItems[vm];
			}

			VM.Items = newItems;

			// Stop editing if the vm associated with that has been removed
			EnsureCurrentlyEditingExists();
		}

		void EnsureCurrentlyEditingExists()
		{
			if (_currentlyEditing != null)
			{
				// Stop if this feature is still present in the new list
				for (int i = 0; i < _manager.Features.Count; i++)
					if (_manager.Features[i] == _currentlyEditing.NativeItem)
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

			void HandleChoice(string choice) => _manager.CreateLocalFeature(choice switch
			{
				"Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
				_ => throw new Exception()
			});
		}

		public void OpenMobileMenu(IProjectFeaturesListItemVM vm) => VM.MobileView = vm;
		public void CloseMobileMenu() => VM.MobileView = null;
		public void MoveUp(IProjectFeaturesListItemVM vm) => _manager.MoveUp(vm.NativeItem);
		public void MoveDown(IProjectFeaturesListItemVM vm) => _manager.MoveDown(vm.NativeItem);
		public void Delete(IProjectFeaturesListItemVM vm) => _manager.Delete(vm.NativeItem);
	}
}
