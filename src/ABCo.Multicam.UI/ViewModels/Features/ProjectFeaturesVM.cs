using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.Structures;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IProjectFeaturesVM : IParameteredService<IProjectFeaturesPresenter>, IAnimationHandlingVM
    {
		IProjectFeaturesListItemVM? MobileView { get; set; }
        IProjectFeaturesListItemVM[] Items { get; set; }
		void CreateFeature(CursorPosition pos);
    }

    public partial class ProjectFeaturesVM : ViewModelBase, IProjectFeaturesVM
    {
		readonly IProjectFeaturesPresenter _presenter;

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

		public ProjectFeaturesVM(IProjectFeaturesPresenter presenter) => _presenter = presenter;
        public void CreateFeature(CursorPosition pos) => _presenter.CreateFeature(pos);
    }
}