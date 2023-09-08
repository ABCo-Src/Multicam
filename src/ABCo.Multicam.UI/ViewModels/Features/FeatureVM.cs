using ABCo.Multicam.Core.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	using AlsoNotify = NotifyPropertyChangedForAttribute;

	public interface IFeatureVM : INotifyPropertyChanged
    {
        public string FeatureTitle { get; set; }
        public string EditBtnText { get; }
        public string EditPanelTitle { get; }
		public bool IsEditing { get; set; }
        ILiveFeatureViewModel? InnerVM { get; set; }
		void ToggleEdit();
        void MoveDown();
        void MoveUp();
        void Delete();
	}

    public interface ILiveFeatureViewModel
    {

    }

    public partial class FeatureVM : ViewModelBase, IFeatureVM
    {
        IProjectFeaturesVM _parent;

        // Synced to the model:
        [ObservableProperty] IFeatureManager _rawManager = null!;
        [ObservableProperty] IFeature _rawFeature = null!;

        [ObservableProperty][AlsoNotify(nameof(EditPanelTitle))] string _featureTitle;
        [ObservableProperty][AlsoNotify(nameof(EditBtnText))] bool _isEditing;
        [ObservableProperty] ILiveFeatureViewModel? _innerVM;

        public FeatureVM() => FeatureTitle = "New Feature";

        public string EditBtnText => IsEditing ? "Finish" : "Edit";
        public string EditPanelTitle => $"Editing '{FeatureTitle}'";

        public void ToggleEdit() => 
            ((IProjectFeaturesVM)_parent).CurrentlyEditing = IsEditing ? null : this; // Will update our editing indicator

        public void MoveDown() => RawManager.MoveDown(_rawFeature);
        public void MoveUp() => RawManager.MoveUp(_rawFeature);
        public void Delete() => RawManager.Delete(_rawFeature);
    }
}