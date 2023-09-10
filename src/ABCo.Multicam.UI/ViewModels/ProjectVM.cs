using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels
{
	public interface IProjectVM 
    { 
        IProjectFeaturesVM Features { get; }
    }

    public partial class ProjectVM : ViewModelBase, IProjectVM
    {
        [ObservableProperty] IProjectFeaturesVM _features;

        public ProjectVM(IServiceSource servSource)
        {
            var presenter = (IProjectFeaturesPresenterForVM)servSource.Get<IFeatureManager>().UIPresenter;
            _features = presenter.VM;
        }
    }
}
