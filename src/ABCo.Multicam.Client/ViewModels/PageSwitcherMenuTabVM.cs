using ABCo.Multicam.Client.Presenters;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Frames
{
	public interface IPageSwitcherMenuTabVM : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsSelected { get; set; }
		IPageVM? AssociatedPage { get; }
		void Select();
	}

    public partial class PageSwitcherMenuTabVM : ViewModelBase, IPageSwitcherMenuTabVM
    {
        public IPageVM? AssociatedPage { get; }

        [ObservableProperty] string _name;
        [ObservableProperty] bool _isSelected;

        readonly IPageSwitcherVM _frame;

		public PageSwitcherMenuTabVM(string name, IPageSwitcherVM frame, IPageVM? associatedPage)
		{
			Name = name;
            AssociatedPage = associatedPage;
            _frame = frame;
		}

        public void Select() => _frame.Select(this);
    }
}
