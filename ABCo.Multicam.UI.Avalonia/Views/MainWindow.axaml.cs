using ABCo.Multicam.UI.ViewModels;
using Avalonia.Controls;

namespace ABCo.Multicam.UI.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Keep the view-model updated about any state changes
    public void MainWindow_Resized(object sender, WindowResizedEventArgs args)
    {
        var vmAsReal = (MainWindowViewModel)DataContext!;
        vmAsReal.IsMaximized = WindowState != WindowState.Maximized;
    }
}
