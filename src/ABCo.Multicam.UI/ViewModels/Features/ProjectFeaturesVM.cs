using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void CreateFeature()
        {
            _dialogHandler.OpenContextMenu(new ContextMenuDetails<FeatureTypes>("Choose Type", RawManager.CreateFeature, null, new ContextMenuItem<FeatureTypes>[]
            {
                new("Switcher", FeatureTypes.Switcher),
                new("Tally", FeatureTypes.Tally)
            }));
        }
    }
}