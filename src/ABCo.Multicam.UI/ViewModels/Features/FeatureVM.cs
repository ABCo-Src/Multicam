using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IFeatureVM : IParameteredService<IFeaturePresenterForVM>, INotifyPropertyChanged
    {
        IFeatureContentVM? Content { get; set; }
        public string FeatureTitle { get; set; }
        public string EditPanelTitle { get; }
        void OnTitleChange();
	}

    public interface IFeatureContentVM
    {

    }

    public partial class FeatureVM : ViewModelBase, IFeatureVM
    {
		readonly IFeaturePresenterForVM _presenter;

        [ObservableProperty] string _featureTitle = "";
        [ObservableProperty] IFeatureContentVM? _content;

		public string EditPanelTitle => $"Editing: {FeatureTitle}";

        public FeatureVM(IFeaturePresenterForVM presenter) => _presenter = presenter;

        public void OnTitleChange() => _presenter.OnTitleChange();
    }
}