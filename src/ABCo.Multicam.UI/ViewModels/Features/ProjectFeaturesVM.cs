using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.Structures;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IProjectFeaturesVM : IParameteredService<IProjectFeaturesPresenterForVM>
    {
        IProjectFeaturesListItemVM[] Items { get; set; }
    }

    public partial class ProjectFeaturesVM : ViewModelBase, IProjectFeaturesVM
    {
		readonly IProjectFeaturesPresenterForVM _presenter;

        [ObservableProperty] IProjectFeaturesListItemVM[] _items = Array.Empty<IProjectFeaturesListItemVM>();

		public ProjectFeaturesVM(IProjectFeaturesPresenterForVM presenter) => _presenter = presenter;
        public void CreateFeature(CursorPosition pos) => _presenter.CreateFeature(pos);
    }
}