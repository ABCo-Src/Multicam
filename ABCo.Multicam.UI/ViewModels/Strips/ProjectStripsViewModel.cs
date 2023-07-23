using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Switchers;
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
        StripViewModel? CurrentlyEditing { get; set; }
        void MoveDown(StripViewModel strip);
        void MoveUp(StripViewModel strip);
        void Delete(StripViewModel strip);
    }

    public partial class ProjectStripsViewModel : ViewModelBase, IProjectStripsViewModel
    {
        IStripManager _manager;
        IServiceSource _servSource;
        IUIDialogHandler _dialogHandler;

        [ObservableProperty] ObservableCollection<StripViewModel> _items;

        public ProjectStripsViewModel(IStripManager manager, IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            _manager = manager;
            _servSource = servSource;
            _items = new ObservableCollection<StripViewModel>();
            _dialogHandler = servSource.Get<IUIDialogHandler>();

            manager.SetStripsChangeForVM(OnStripsChange);
            OnStripsChange();
        }

        StripViewModel? _currentlyEditing;
        public StripViewModel? CurrentlyEditing
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
            var oldItems = new List<StripViewModel>(Items);
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

        StripViewModel CreateVMForStrip(IRunningStrip strip) => strip switch
        {
            ISwitcherRunningStrip switchingStrip => new SwitcherStripViewModel(switchingStrip, _servSource, this),
            _ => new UnsupportedStripViewModel(strip, _servSource, this),
        };


        public void CreateStrip()
        {
            // Temporary test
            _dialogHandler.OpenSimpleContext("Choose Type", a => Debugger.Break(), Debugger.Break, new ContextMenuItem<int>[]
            {
                new() { Name = "First Item", Value = 1 },
                new() { Name = "Second Item", Value = 2 }
            });

            _manager.CreateStrip();
        }

        public void MoveDown(StripViewModel strip) => _manager.MoveDown(strip.BaseStrip);
        public void MoveUp(StripViewModel strip) => _manager.MoveUp(strip.BaseStrip);
        public void Delete(StripViewModel strip) => _manager.Delete(strip.BaseStrip);
    }
}