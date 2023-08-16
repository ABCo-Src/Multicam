using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels;

public interface IApplicationVM 
{ 
    IProjectVM Project { get; }
}

public partial class ApplicationVM : ViewModelBase, IApplicationVM
{
    [ObservableProperty]IProjectVM _project;

    public ApplicationVM(IServiceSource servSource)
    {
        Project = servSource.Get<IProjectVM>();
    }
}