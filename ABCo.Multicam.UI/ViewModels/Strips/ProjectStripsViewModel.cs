using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Structures;
using ABCo.Multicam.Tests.UI;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
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

namespace ABCo.Multicam.UI.ViewModels.Strips
{
    public interface IProjectStripsViewModel 
    {
        IStripViewModel? CurrentlyEditing { get; set; }
        void MoveDown(IStripViewModel strip);
        void MoveUp(IStripViewModel strip);
        void Delete(IStripViewModel strip);
    }

    public partial class ProjectStripsViewModel : ViewModelBase, IProjectStripsViewModel
    {
        IStripManager _manager;
        IServiceSource _servSource;
        IUIDialogHandler _dialogHandler;

        [ObservableProperty] ObservableCollection<IStripViewModel> _items;

        public ProjectStripsViewModel(IStripManager manager, IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            _manager = manager;
            _servSource = servSource;
            _items = new ObservableCollection<IStripViewModel>();
            _dialogHandler = servSource.Get<IUIDialogHandler>();

            manager.SetStripsChangeForVM(OnStripsChange);
            OnStripsChange();
        }

        IStripViewModel? _currentlyEditing;
        public IStripViewModel? CurrentlyEditing
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

        void OnStripsChange()
        {
            // Clear the old strips
            var oldItems = new List<IStripViewModel>(Items);
            Items.Clear();
            
            // Re-add them
            var baseItems = _manager.Strips;
            for (int i = 0; i < baseItems.Count; i++)
            {
                // Re-use or create a new vm
                int vm = oldItems.FindIndex(s => s.BaseStrip == baseItems[i]);

                if (vm == -1)
                    Items.Add(CreateVMForStrip(baseItems[i]));
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

        IStripViewModel CreateVMForStrip(IRunningStrip strip) => strip switch
        {
            ISwitcherRunningStrip switchingStrip => _servSource.GetWithParameter<ISwitcherStripViewModel, StripViewModelInfo>(new StripViewModelInfo(strip, this)),
            _ => new UnsupportedStripViewModel(new StripViewModelInfo(strip, this), _servSource),
        };


        public void CreateStrip()
        {
            // Temporary test
            _dialogHandler.OpenContextMenu(new ContextMenuDetails<StripTypes>("Choose Type", _manager.CreateStrip, null, new ContextMenuItem<StripTypes>[]
            {
                new("Switcher", StripTypes.Switcher),
                new("Tally", StripTypes.Tally)
            }));
        }

        public void MoveDown(IStripViewModel strip) => _manager.MoveDown(strip.BaseStrip);
        public void MoveUp(IStripViewModel strip) => _manager.MoveUp(strip.BaseStrip);
        public void Delete(IStripViewModel strip) => _manager.Delete(strip.BaseStrip);
    }

    public record struct StripViewModelInfo(IRunningStrip Strip, IProjectStripsViewModel Parent);
}