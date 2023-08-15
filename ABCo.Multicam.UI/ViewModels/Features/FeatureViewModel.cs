using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
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
    public interface IFeatureViewModel : IVMForFeatureBinder
    {
        public abstract IFeatureContainer BaseFeature { get; }
        public bool IsEditing { get; set; }
    }

    public partial class FeatureViewModel : BindingViewModelBase<IVMForFeatureBinder>, IVMForFeatureBinder, IFeatureViewModel
    {
        // Synced to the model:
        [ObservableProperty] IFeatureManager _rawManager;
        [ObservableProperty] IFeatureContainer _rawFeature;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditPanelTitle))] string _featureTitle;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{FeatureTitle}'";

        public IFeatureContainer BaseFeature { get; }
        public FeatureViewType ContentView { get; }

        public FeatureViewModel()
        {
            FeatureTitle = "New Feature";
        }

        public void ToggleEdit() => 
            ((IProjectFeaturesViewModel)Parent).CurrentlyEditing = IsEditing ? null : this; // Will update our editing indicator

        public void MoveDown() => RawManager.MoveDown(RawFeature);
        public void MoveUp() => RawManager.MoveUp(RawFeature);
        public void Delete() => RawManager.Delete(RawFeature);
    }

    public interface IUnsupportedFeatureViewModel : IFeatureViewModel { }
    public class UnsupportedFeatureViewModel : ViewModelBase
    {
        ILiveFeature _feature;

        public UnsupportedFeatureViewModel(NewViewModelInfo info, IServiceSource serviceSource)// : base(serviceSource, (IProjectFeaturesViewModel)info.Parent) 
            => _feature = (ILiveFeature)info.Model;

        //public override IRunningFeature BaseFeature => _feature;
        //public override FeatureViewType ContentView => FeatureViewType.Unsupported;
    }
}