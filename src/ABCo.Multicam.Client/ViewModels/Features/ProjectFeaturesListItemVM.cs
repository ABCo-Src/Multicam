using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.ViewModels.Features
{
    public interface ISwitcherListItemVM : IClientService<ISwitcherListVM, IFeature, IFeatureVM>, INotifyPropertyChanged
	{
		IFeature NativeItem { get; }
		IFeatureVM Feature { get; }
		string EditBtnText { get; set; }
		void MoveDown();
		void MoveUp();
		void Delete();
	}

	public partial class SwitcherListItemVM : ViewModelBase, ISwitcherListItemVM
	{
		public IFeature NativeItem { get; }

		readonly ISwitcherListVM _presenter;
		[ObservableProperty] IFeatureVM _feature;
		[ObservableProperty] string _editBtnText = "";

		public SwitcherListItemVM(ISwitcherListVM presenter, IFeature nativeItem, IFeatureVM innerVM)
		{
			NativeItem = nativeItem;
			_presenter = presenter;
			_feature = innerVM;
		}

		public void MoveDown() => _presenter.MoveDown(this);
		public void MoveUp() => _presenter.MoveUp(this);
		public void Delete() => _presenter.Delete(this);
	}
}
