using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels;

public interface IApplicationViewModel 
{ 
    IProjectViewModel Project { get; }
}

public partial class ApplicationViewModel : ViewModelBase, IApplicationViewModel
{
    [ObservableProperty]IProjectViewModel _project;

    public ApplicationViewModel(IServiceSource servSource)
    {
        Project = servSource.Get<IProjectViewModel>();
    }
}