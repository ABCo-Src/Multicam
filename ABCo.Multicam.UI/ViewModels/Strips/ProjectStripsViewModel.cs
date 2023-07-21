using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Tests.UI;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        [ObservableProperty] ObservableCollection<StripViewModel> _items;
        
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

        public void CreateStrip()
        {
            _manager.CreateStrip();
            //Items.Add(new SwitcherStripViewModel(_servSource, this));
        }

        public void EditStrip(StripViewModel vm)
        {
            CurrentlyEditing = vm;
        }

        public ProjectStripsViewModel(IStripManager manager, IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            _manager = manager;
            _servSource = servSource;
            _items = new ObservableCollection<StripViewModel>();
        }

        public void MoveDown(StripViewModel strip)
        {
            
        }

        public void MoveUp(StripViewModel strip)
        {
            
        }

        public void Delete(StripViewModel strip)
        {
            
        }
    }
}