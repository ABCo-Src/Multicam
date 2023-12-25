using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.ViewModels.Paging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Frames
{
    public interface IHomePageLinkVM : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsSelected { get; set; }
		IPageVM? AssociatedPage { get; }
		void Select();
	}

    public partial class HomePageLinkVM : ViewModelBase, IHomePageLinkVM
    {
        public IPageVM? AssociatedPage { get; }

        [ObservableProperty] string _name;
        [ObservableProperty] bool _isSelected;

        readonly IHomeVM _homePage;

		public HomePageLinkVM(string name, IHomeVM frame, IPageVM? associatedPage)
		{
			Name = name;
            AssociatedPage = associatedPage;
            _homePage = frame;
		}

        public void Select() => _homePage.Select(this);
    }
}
