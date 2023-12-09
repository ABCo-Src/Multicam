using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Structures;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.ViewModels.Features
{
    public interface ISwitcherListVM : IClientService<ISwitcherCollectionPresenter>, IPageVM, INotifyPropertyChanged
    {
		ISwitcherListItemVM? MobileView { get; set; }
        ISwitcherListItemVM[] Items { get; set; }
		void CreateFeature(CursorPosition pos);
    }

    public partial class SwitcherListVM : ViewModelBase, ISwitcherListVM
    {
		readonly ISwitcherCollectionPresenter _presenter;

        public ISwitcherListItemVM? MobileView { get; set; }

		public AppPages Page => AppPages.Switchers;

		[ObservableProperty] ISwitcherListItemVM[] _items = Array.Empty<ISwitcherListItemVM>();

		public SwitcherListVM(ISwitcherCollectionPresenter presenter) => _presenter = presenter;
        public void CreateFeature(CursorPosition pos) => _presenter.CreateFeature(pos);
    }
}