using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features;
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
        [ObservableProperty] IProjectFeaturesViewModel _features;

        public ProjectViewModel(IServiceSource servSource)
        {
            if (servSource == null) throw new ServiceSourceNotGivenException();

            var binder = (IVMBinder<IVMForProjectFeaturesBinder>)servSource.Get<IFeatureManager>().VMBinder;
            _features = binder.GetVM<IProjectFeaturesViewModel>(this);
        }
    }
}
