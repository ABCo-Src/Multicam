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
    public interface IStripViewModel 
    {
        public abstract IRunningStrip BaseStrip { get; }
        public bool IsEditing { get; set; }
    }

    public abstract partial class StripViewModel : ViewModelBase, IStripViewModel
    {
        protected IServiceSource _serviceSource;
        public readonly IProjectStripsViewModel Parent;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditPanelTitle))] string _stripTitle;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{StripTitle}'";

        public abstract IRunningStrip BaseStrip { get; }
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

    public interface IUnsupportedStripViewModel : IStripViewModel { }
    public class UnsupportedStripViewModel : StripViewModel, IUnsupportedStripViewModel
    {
        IRunningStrip _strip;

        public UnsupportedStripViewModel(StripViewModelInfo info, IServiceSource serviceSource)
            : base(serviceSource, info.Parent) => _strip = info.Strip;

        public override IRunningStrip BaseStrip => _strip;
        public override StripViewType ContentView => StripViewType.Unsupported;
    }
}