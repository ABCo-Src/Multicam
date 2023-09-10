using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.Structures;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
	public interface IProjectFeaturesPresenterForVM : IProjectFeaturesPresenter, IParameteredService<IFeatureManager>
	{
		IProjectFeaturesVM VM { get; }
		void ToggleEdit(IProjectFeaturesListItemVM vm);
		void MoveUp(IProjectFeaturesListItemVM vm);
		void MoveDown(IProjectFeaturesListItemVM vm);
		void Delete(IProjectFeaturesListItemVM vm);
		void CreateFeature(CursorPosition pos);
	}

	public class ProjectFeaturesPresenter : IProjectFeaturesPresenterForVM
	{
		readonly IFeatureManager _manager;
		readonly IServiceSource _servSource;
		readonly IUIDialogHandler _dialogHandler;

		public IProjectFeaturesVM VM { get; }
		IProjectFeaturesListItemVM? _currentlyEditing;

		public static IProjectFeaturesPresenter New(IFeatureManager manager, IServiceSource servSource) => new ProjectFeaturesPresenter(manager, servSource);
		public ProjectFeaturesPresenter(IFeatureManager manager, IServiceSource servSource)
		{
			_manager = manager;
			_currentlyEditing = null;
			_dialogHandler = servSource.Get<IUIDialogHandler>();
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
					DisableEditing(newItems[i]);
				}
				else
					newItems[i] = oldItems[vm];
			}

			VM.Items = newItems;

			// Stop editing if the vm associated with that has been removed
			if (_currentlyEditing != null) 
				EnsureCurrentlyEditingExists();
		}

		void EnsureCurrentlyEditingExists()
		{
			// Stop if this feature is still present in the new list
			for (int i = 0; i < _manager.Features.Count; i++)
				if (_manager.Features[i] == _currentlyEditing)
					return;

			// Stop editing if we weren't stopped
			ChangeCurrentlyEditing(null);
		}

		void ChangeCurrentlyEditing(IProjectFeaturesListItemVM? newValue)
		{
			var oldValue = _currentlyEditing;
			VM.ShowEditingPanel = newValue != null;

			_currentlyEditing = newValue;

			// Update the list item's VM to match this
			if (oldValue != null) DisableEditing(oldValue);
			if (newValue != null) EnableEditing(newValue);

			VM.CurrentlyEditing = _currentlyEditing;
		}

		void DisableEditing(IProjectFeaturesListItemVM vm) => vm.EditBtnText = "Edit";
		void EnableEditing(IProjectFeaturesListItemVM vm) => vm.EditBtnText = "Finish";

		public void ToggleEdit(IProjectFeaturesListItemVM vm) => ChangeCurrentlyEditing(_currentlyEditing == null ? vm : null);

		public void CreateFeature(CursorPosition pos)
		{
			_dialogHandler.OpenContextMenu(new ContextMenuDetails("Choose Type", HandleChoice, null, pos, new string[]
			{
				"Switcher",
				"Tally"
			}));

			void HandleChoice(string choice) => _manager.CreateFeature(choice switch
			{
				"Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
				_ => throw new Exception()
			});
		}

		public void MoveUp(IProjectFeaturesListItemVM vm) => _manager.MoveUp(vm.NativeItem);
		public void MoveDown(IProjectFeaturesListItemVM vm) => _manager.MoveDown(vm.NativeItem);
		public void Delete(IProjectFeaturesListItemVM vm) => _manager.Delete(vm.NativeItem);
	}
}
