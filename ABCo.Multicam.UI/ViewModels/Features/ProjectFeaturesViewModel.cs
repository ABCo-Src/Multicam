using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Tests;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Helpers;
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
    public interface IProjectFeaturesViewModel : IVMForProjectFeaturesBinder
    {
        IFeatureViewModel? CurrentlyEditing { get; set; }
    }

    public partial class ProjectFeaturesViewModel : BindingViewModelBase<IVMForProjectFeaturesBinder>, IProjectFeaturesViewModel
    {
        IServiceSource _servSource;
        IUIDialogHandler _dialogHandler;

        // Raw data synced to the model:
        IVMBinder<IVMForFeatureBinder>[] _rawFeatures;
        public IVMBinder<IVMForFeatureBinder>[] RawFeatures
        {
            get => _rawFeatures;
            set
            {
                // Update the property
                if (SetProperty(ref _rawFeatures, value, nameof(RawFeatures)))
                    OnPropertyChanged(nameof(Items));

                // Update "CurrentlyEditing" if needed
                EnsureCurrentlyEditingExists();
            }
        }

        [ObservableProperty] IFeatureManager _rawManager = null!;

        public IEnumerable<IFeatureViewModel> Items => RawFeatures.Select(f => f.GetVM<IFeatureViewModel>(this));

        public ProjectFeaturesViewModel(IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            _servSource = servSource;
            _dialogHandler = servSource.Get<IUIDialogHandler>();

            _rawFeatures = Array.Empty<IVMBinder<IVMForFeatureBinder>>();
            _rawManager = null!;
        }

        IFeatureViewModel? _currentlyEditing;
        public IFeatureViewModel? CurrentlyEditing
        {
            get => _currentlyEditing;
            set
            {
                // If there was a previous item, reset that
                if (_currentlyEditing != null)
                    _currentlyEditing.IsEditing = false;

                // If there's a new item, assign editing on that
                if (value != null) 
                    value.IsEditing = true;

                SetProperty(ref _currentlyEditing, value);

                // Notify change in affected
                OnPropertyChanged(nameof(ShowEditingPanel));
            }
        }

        public bool ShowEditingPanel => CurrentlyEditing != null;

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