using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features
{
    using AlsoNotify = NotifyPropertyChangedForAttribute;

    public interface IFeatureViewModel : IVMForFeatureBinder
    {
        public bool IsEditing { get; set; }
    }

    public interface ILiveFeatureViewModel
    {

    }

    public partial class FeatureViewModel : BindingViewModelBase<IVMForFeatureBinder>, IVMForFeatureBinder, IFeatureViewModel
    {
        // Synced to the model:
        [ObservableProperty] IFeatureManager _rawManager = null!;
        [ObservableProperty] IFeatureContainer _rawContainer = null!;
        [ObservableProperty][AlsoNotify(nameof(InnerType))][AlsoNotify(nameof(InnerVM))] ILiveFeature _rawInnerFeature = null!;

        [ObservableProperty][AlsoNotify(nameof(EditPanelTitle))] string _featureTitle;
        [ObservableProperty][AlsoNotify(nameof(EditBtnText))] bool _isEditing;

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{FeatureTitle}'";

        public FeatureTypes InnerType => RawInnerFeature.FeatureType;

        public ILiveFeatureViewModel InnerVM => InnerType switch 
        {
            FeatureTypes.Switcher => ((IVMBinder<IVMForSwitcherFeature>)RawInnerFeature.UIBinder).GetVM<ISwitcherFeatureVM>(this),
            _ => new UnsupportedFeatureViewModel()
        };

        public FeatureViewModel()
        {
            FeatureTitle = "New Feature";
        }

        public void ToggleEdit() => 
            ((IProjectFeaturesViewModel)Parent).CurrentlyEditing = IsEditing ? null : this; // Will update our editing indicator

        public void MoveDown() => RawManager.MoveDown(RawContainer);
        public void MoveUp() => RawManager.MoveUp(RawContainer);
        public void Delete() => RawManager.Delete(RawContainer);
    }
}