using ABCo.Multicam.Core;
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
    public partial class StripViewModel : ViewModelBase, IStripViewModel
    {
        public readonly IProjectStripsViewModel Parent;
        [ObservableProperty] string _stripTitle;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";

        public StripViewModel(IServiceSource manager, IProjectStripsViewModel parent)
        {
            if (manager == null) throw new ServiceSourceNotGivenException();

            StripTitle = "New Strip";
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