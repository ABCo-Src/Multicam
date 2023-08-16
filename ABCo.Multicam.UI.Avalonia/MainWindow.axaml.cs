using ABCo.Multicam.UI.ViewModels;
using Avalonia.Controls;

namespace ABCo.Multicam.UI.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Keep the view-model updated about any state changes
    public void MainWindow_Resized(object sender, WindowResizedEventArgs args)
    {
        var vmAsReal = (MainWindowVM)DataContext!;
        vmAsReal.IsMaximized = WindowState != WindowState.Maximized;
    }
}
