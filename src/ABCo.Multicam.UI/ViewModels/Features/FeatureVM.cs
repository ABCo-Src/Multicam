using ABCo.Multicam.Server;
using ABCo.Multicam.UI.Presenters.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IFeatureVM : IClientService<IMainFeaturePresenter>, INotifyPropertyChanged
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
		readonly IMainFeaturePresenter _presenter;

        [ObservableProperty] string _featureTitle = "";
        [ObservableProperty] IFeatureContentVM? _content;

		public string EditPanelTitle => $"Editing: {FeatureTitle}";

        public FeatureVM(IMainFeaturePresenter presenter) => _presenter = presenter;

        public void OnTitleChange() => _presenter.OnTitleChange();
    }
}