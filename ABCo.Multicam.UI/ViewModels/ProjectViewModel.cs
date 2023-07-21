using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Strips;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels
{
    public interface IProjectViewModel { }
    public partial class ProjectViewModel : ViewModelBase, IProjectViewModel
    {
        [ObservableProperty] ProjectStripsViewModel _strips;

        public ProjectViewModel(IServiceSource manager)
        {
            if (manager == null) throw new ServiceSourceNotGivenException();
            _strips = new ProjectStripsViewModel(manager.Get<IStripManager>(), manager);
        }
    }
}
