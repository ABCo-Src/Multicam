using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips
{
    public interface IStripViewModel { }
    public abstract partial class StripViewModel : ViewModelBase, IStripViewModel
    {
        protected IServiceSource _serviceSource;
        public readonly IProjectStripsViewModel Parent;

        public abstract IRunningStrip BaseStrip { get; }

        [ObservableProperty] string _stripTitle;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";

        public abstract StripViewType ContentView { get; }

        public StripViewModel(IServiceSource serviceSource, IProjectStripsViewModel parent)
        {
            if (serviceSource == null) throw new ServiceSourceNotGivenException();

            StripTitle = "New Strip";
            _serviceSource = serviceSource;
            Parent = parent;
        }

        public void ToggleEdit()
        {
            if (IsEditing)
                Parent.CurrentlyEditing = null;
            else
                Parent.CurrentlyEditing = this; // Will update our editing indicator
        }

        public void MoveDown() => Parent.MoveDown(this);
        public void MoveUp() => Parent.MoveUp(this);
        public void Delete() => Parent.Delete(this);
    }
}