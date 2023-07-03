using ABCo.Multicam.Core;
using ABCo.Multicam.Tests.UI;
using ABCo.Multicam.UI.Helpers;
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

        IServiceSource _manager;

        public void AddStrip()
        {
            Items.Add(new StripViewModel(_manager, this));
        }

        public void EditStrip(StripViewModel vm)
        {
            CurrentlyEditing = vm;
        }

        public ProjectStripsViewModel(IServiceSource manager)
        {
            if (manager == null) throw new ServiceSourceNotGivenException();

            _manager = manager;
            _items = new ObservableCollection<StripViewModel>();
        }

        public void MoveDown(StripViewModel strip)
        {
            int indexOfStrip = Items.IndexOf(strip);

            // Don't do anything if it's at the end
            if (indexOfStrip == Items.Count - 1) return;

            (Items[indexOfStrip], Items[indexOfStrip + 1]) = (Items[indexOfStrip + 1], Items[indexOfStrip]);
        }

        public void MoveUp(StripViewModel strip)
        {
            int indexOfStrip = Items.IndexOf(strip);

            // Don't do anything if it's at the start
            if (indexOfStrip == 0) return;

            (Items[indexOfStrip], Items[indexOfStrip - 1]) = (Items[indexOfStrip - 1], Items[indexOfStrip]);
        }

        public void Delete(StripViewModel strip)
        {
            Items.Remove(strip);
            if (strip == CurrentlyEditing) CurrentlyEditing = null;
        }
    }
}