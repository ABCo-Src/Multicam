using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
    using AlsoNotify = NotifyPropertyChangedForAttribute;

    public interface IFeatureVM : IVMForFeatureBinder, INotifyPropertyChanged
    {
        public string FeatureTitle { get; set; }
        public string EditBtnText { get; }
        public string EditPanelTitle { get; }
		public bool IsEditing { get; set; }
        ILiveFeatureViewModel? InnerVM { get; set; }
        FeatureTypes InnerType { get; }
		void ToggleEdit();
        void MoveDown();
        void MoveUp();
        void Delete();
	}

    public interface ILiveFeatureViewModel
    {

    }

    public partial class FeatureVM : BindingViewModelBase<IVMForFeatureBinder>, IVMForFeatureBinder, IFeatureVM
    {
        // Synced to the model:
        [ObservableProperty] IFeatureManager _rawManager = null!;
        [ObservableProperty] IFeatureContainer _rawContainer = null!;
        [ObservableProperty][AlsoNotify(nameof(InnerType))] ILiveFeature _rawInnerFeature = null!;

        [ObservableProperty][AlsoNotify(nameof(EditPanelTitle))] string _featureTitle;
        [ObservableProperty][AlsoNotify(nameof(EditBtnText))] bool _isEditing;
        [ObservableProperty] ILiveFeatureViewModel? _innerVM;

        public FeatureVM() => FeatureTitle = "New Feature";

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{FeatureTitle}'";

        public FeatureTypes InnerType => RawInnerFeature.FeatureType;

        partial void OnRawInnerFeatureChanged(ILiveFeature value)
        {
            InnerVM = value.FeatureType switch
            {
                FeatureTypes.Switcher => ((IVMBinder<IVMForSwitcherFeature>)RawInnerFeature.UIBinder).GetVM<ISwitcherFeatureVM>(this),
                _ => new UnsupportedFeatureVM()
            };
        }

        public void ToggleEdit() => 
            ((IProjectFeaturesVM)Parent).CurrentlyEditing = IsEditing ? null : this; // Will update our editing indicator

        public void MoveDown() => RawManager.MoveDown(RawContainer);
        public void MoveUp() => RawManager.MoveUp(RawContainer);
        public void Delete() => RawManager.Delete(RawContainer);
    }
}