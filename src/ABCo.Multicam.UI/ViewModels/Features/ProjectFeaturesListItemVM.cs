using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Presenters;
using ABCo.Multicam.UI.Presenters.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IProjectFeaturesListItemVM : IParameteredService<IProjectFeaturesPresenterForVM, IFeature, IFeatureVM>, INotifyPropertyChanged, ISideMenuEmbeddableVM
	{
		IFeature NativeItem { get; }
		IFeatureVM Feature { get; }
		string EditBtnText { get; set; }
		void ToggleEdit();
		void MoveDown();
		void MoveUp();
		void Delete();
	}

	public partial class ProjectFeaturesListItemVM : ViewModelBase, IProjectFeaturesListItemVM
	{
		public IFeature NativeItem { get; }

		readonly IProjectFeaturesPresenterForVM _presenter;
		[ObservableProperty] IFeatureVM _feature;
		[ObservableProperty] string _editBtnText = "";

		public ProjectFeaturesListItemVM(IProjectFeaturesPresenterForVM presenter, IFeature nativeItem, IFeatureVM innerVM)
		{
			NativeItem = nativeItem;
			_presenter = presenter;
			_feature = innerVM;
		}

		public void ToggleEdit() => _presenter.ToggleEdit(this);
		public void MoveDown() => _presenter.MoveDown(this);
		public void MoveUp() => _presenter.MoveUp(this);
		public void Delete() => _presenter.Delete(this);
	}
}
