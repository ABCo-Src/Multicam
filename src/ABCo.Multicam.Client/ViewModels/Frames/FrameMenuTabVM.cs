using ABCo.Multicam.Client.Presenters;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Frames
{
    public interface IFrameMenuTabVM : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsSelected { get; set; }
		IPageVM? AssociatedPage { get; }
		void Select();
	}

    public partial class FrameMenuTabVM : ViewModelBase, IFrameMenuTabVM
    {
        public IPageVM? AssociatedPage { get; }

        [ObservableProperty] string _name;
        [ObservableProperty] bool _isSelected;

        readonly IFrameUIPresenter _presenter;

		public FrameMenuTabVM(string name, IFrameUIPresenter presenter, IPageVM? associatedPage)
		{
			Name = name;
            AssociatedPage = associatedPage;
            _presenter = presenter;
		}

        public void Select() => _presenter.Select(this);
    }
}
