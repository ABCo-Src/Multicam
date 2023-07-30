using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Tests;
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
    public interface IProjectFeaturesViewModel 
    {
        IFeatureViewModel? CurrentlyEditing { get; set; }
        void MoveDown(IFeatureViewModel feature);
        void MoveUp(IFeatureViewModel feature);
        void Delete(IFeatureViewModel feature);
    }

    public partial class ProjectFeaturesViewModel : ViewModelBase, IProjectFeaturesViewModel
    {
        IFeatureManager _manager;
        IServiceSource _servSource;
        IUIDialogHandler _dialogHandler;

        [ObservableProperty] ObservableCollection<IFeatureViewModel> _items;

        public ProjectFeaturesViewModel(IFeatureManager manager, IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            _manager = manager;
            _servSource = servSource;
            _items = new ObservableCollection<IFeatureViewModel>();
            _dialogHandler = servSource.Get<IUIDialogHandler>();

            manager.SetOnFeaturesChangeForVM(OnFeaturesChange);
            OnFeaturesChange();
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

        void OnFeaturesChange()
        {
            // Clear the old features
            var oldItems = new List<IFeatureViewModel>(Items);
            Items.Clear();
            
            // Re-add them
            var baseItems = _manager.Features;
            for (int i = 0; i < baseItems.Count; i++)
            {
                // Re-use or create a new vm
                int vm = oldItems.FindIndex(s => s.BaseFeature == baseItems[i]);

                if (vm == -1)
                    Items.Add(CreateVMForFeature(baseItems[i]));
                else
                {
                    Items.Add(oldItems[vm]);
                    oldItems.RemoveAt(vm);
                }
            }

            // If we were editing a removed vm, deselect it
            for (int i = 0; i < oldItems.Count; i++)
                if (oldItems[i] == CurrentlyEditing)
                    CurrentlyEditing = null;
        }

        IFeatureViewModel CreateVMForFeature(IRunningFeature feature) => feature switch
        {
            ISwitcherRunningFeature => _servSource.GetVM<ISwitcherFeatureVM>(new(feature, this)),
            _ => new UnsupportedFeatureViewModel(new(feature, this), _servSource),
        };

        public void CreateFeature()
        {
            _dialogHandler.OpenContextMenu(new ContextMenuDetails<FeatureTypes>("Choose Type", _manager.CreateFeature, null, new ContextMenuItem<FeatureTypes>[]
            {
                new("Switcher", FeatureTypes.Switcher),
                new("Tally", FeatureTypes.Tally)
            }));
        }

        public void MoveDown(IFeatureViewModel feature) => _manager.MoveDown(feature.BaseFeature);
        public void MoveUp(IFeatureViewModel feature) => _manager.MoveUp(feature.BaseFeature);
        public void Delete(IFeatureViewModel feature) => _manager.Delete(feature.BaseFeature);
    }
}