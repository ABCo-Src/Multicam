using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features
{
    public interface IFeatureViewModel 
    {
        public abstract IRunningFeature BaseFeature { get; }
        public bool IsEditing { get; set; }
    }

    public abstract partial class FeatureViewModel : ViewModelBase, IFeatureViewModel
    {
        protected IServiceSource _serviceSource;
        public readonly IProjectFeaturesViewModel Parent;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditPanelTitle))] string _featureTitle;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{FeatureTitle}'";

        public abstract IRunningFeature BaseFeature { get; }
        public abstract FeatureViewType ContentView { get; }

        public FeatureViewModel(IServiceSource serviceSource, IProjectFeaturesViewModel parent)
        {
            if (serviceSource == null) throw new ServiceSourceNotGivenException();

            FeatureTitle = "New Feature";
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

    public interface IUnsupportedFeatureViewModel : IFeatureViewModel { }
    public class UnsupportedFeatureViewModel : FeatureViewModel, IUnsupportedFeatureViewModel
    {
        IRunningFeature _feature;

        public UnsupportedFeatureViewModel(FeatureViewModelInfo info, IServiceSource serviceSource)
            : base(serviceSource, info.Parent) => _feature = info.Feature;

        public override IRunningFeature BaseFeature => _feature;
        public override FeatureViewType ContentView => FeatureViewType.Unsupported;
    }
}