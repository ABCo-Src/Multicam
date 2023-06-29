using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels;

public interface IApplicationViewModel { }
public partial class ApplicationViewModel : ViewModelBase, IApplicationViewModel
{
    [ObservableProperty]IProjectViewModel _project;

    public ApplicationViewModel(IProjectViewModel project)
    {
        Project = project;
    }
}