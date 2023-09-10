using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.Structures;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IProjectFeaturesVM : IParameteredService<IProjectFeaturesPresenterForVM>
    {
		IProjectFeaturesListItemVM? CurrentlyEditing { get; set; }
        IProjectFeaturesListItemVM[] Items { get; set; }
        bool ShowEditingPanel { get; set; }
    }

    public partial class ProjectFeaturesVM : ViewModelBase, IProjectFeaturesVM
    {
		readonly IProjectFeaturesPresenterForVM _presenter;

        public static IProjectFeaturesVM New(IProjectFeaturesPresenterForVM presenter, IServiceSource servSource) => new ProjectFeaturesVM(presenter);
		public ProjectFeaturesVM(IProjectFeaturesPresenterForVM presenter) => _presenter = presenter;

        [ObservableProperty] IProjectFeaturesListItemVM[] _items = Array.Empty<IProjectFeaturesListItemVM>();
        [ObservableProperty] IProjectFeaturesListItemVM? _currentlyEditing;
        [ObservableProperty] bool _showEditingPanel;

        public void CreateFeature(CursorPosition pos) => _presenter.CreateFeature(pos);
    }
}