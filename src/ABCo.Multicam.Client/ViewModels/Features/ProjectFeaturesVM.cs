using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.Structures;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features
{
	public interface IProjectFeaturesVM : IClientService<IMainFeatureCollectionPresenter>, IAnimationHandlingVM
    {
		IProjectFeaturesListItemVM? MobileView { get; set; }
        IProjectFeaturesListItemVM[] Items { get; set; }
		void CreateFeature(CursorPosition pos);
    }

    public partial class ProjectFeaturesVM : ViewModelBase, IProjectFeaturesVM
    {
		readonly IMainFeatureCollectionPresenter _presenter;

		IProjectFeaturesListItemVM? _mobileView;
        public IProjectFeaturesListItemVM? MobileView
        {
            get => _mobileView;
			set => UpdateMobileView(value);
		}

		public async void UpdateMobileView(IProjectFeaturesListItemVM? newVal)
		{
			await WaitForAnimationHandler(nameof(MobileView));
			_mobileView = newVal;
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(MobileView)));
		}

		[ObservableProperty] IProjectFeaturesListItemVM[] _items = Array.Empty<IProjectFeaturesListItemVM>();

		public ProjectFeaturesVM(IMainFeatureCollectionPresenter presenter) => _presenter = presenter;
        public void CreateFeature(CursorPosition pos) => _presenter.CreateFeature(pos);
    }
}