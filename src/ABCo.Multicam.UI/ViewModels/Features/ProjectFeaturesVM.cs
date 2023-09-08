using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.Structures;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IProjectFeaturesVM : IVMForProjectFeaturesBinder
    {
        IFeatureVM? CurrentlyEditing { get; set; }
    }

    public partial class ProjectFeaturesVM : BindingViewModelBase<IVMForProjectFeaturesBinder>, IProjectFeaturesVM
    {
        IUIDialogHandler _dialogHandler;

        public ProjectFeaturesVM(IServiceSource servSource) => _dialogHandler = servSource.Get<IUIDialogHandler>();

        // Raw data synced to the model:
        [ObservableProperty] IVMBinder<IVMForFeatureBinder>[] _rawFeatures = null!;
        [ObservableProperty] IFeatureManager _rawManager = null!;

        [ObservableProperty] IFeatureVM[]? _items;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(ShowEditingPanel))] IFeatureVM? _currentlyEditing;

        public bool ShowEditingPanel => CurrentlyEditing != null;

        partial void OnCurrentlyEditingChanged(IFeatureVM? oldValue, IFeatureVM? newValue)
        {
            // Reset the old and assign the new (if needed)
            if (oldValue != null) oldValue.IsEditing = false;
            if (newValue != null) newValue.IsEditing = true;
        }

        partial void OnRawFeaturesChanged(IVMBinder<IVMForFeatureBinder>[] value)
        {
            UpdateItems();
            EnsureCurrentlyEditingExists();
        }

        public void UpdateItems()
        {
            var newItems = new IFeatureVM[RawFeatures.Length];
            for (int i = 0; i < newItems.Length; i++) newItems[i] = RawFeatures[i].GetVM<IFeatureVM>(this);
            Items = newItems;
        }

        void EnsureCurrentlyEditingExists()
        {
            if (CurrentlyEditing == null) return;
            var currentlyEditingBinder = CurrentlyEditing.Binder;

            // Stop if this feature is still present in the new list
            for (int i = 0; i < RawFeatures.Length; i++)
                if (RawFeatures[i] == currentlyEditingBinder)
                    return;

            // Stop editing if we weren't stopped
            CurrentlyEditing = null;
        }

        public void CreateFeature(CursorPosition pos)
        {
            _dialogHandler.OpenContextMenu(new ContextMenuDetails("Choose Type", HandleChoice, null, pos, new string[]
            {
                "Switcher",
                "Tally"
            }));

            void HandleChoice(string choice) => RawManager.CreateFeature(choice switch
            {
                "Switcher" => FeatureTypes.Switcher,
				"Tally" => FeatureTypes.Tally,
                _ => throw new Exception()
			});
        }
    }
}