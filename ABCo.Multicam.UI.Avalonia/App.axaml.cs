using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.Avalonia.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ABCo.Multicam.UI.Avalonia.Controls.Window;
using ABCo.Multicam.UI.Services;
using LightInject;
using System.Diagnostics;

namespace ABCo.Multicam.UI.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // Desktop (Windows, Mac OS, Linux)
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create the window
            var newWindow = new MainWindow();

            // Initialize the DI container
            var container = UIAvaloniaStatics.InitializeContainer(newWindow, newWindow.AppContent);

            // Use this to create the main VM + finish
            newWindow.DataContext = container.GetInstance(typeof(MainWindowViewModel));
            desktop.MainWindow = newWindow;
        }

        // Web
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // Create the main window view
            var mainWindowView = new MainWindowView();

            // Initialize the DI container
            var container = UIAvaloniaStatics.InitializeContainer(null, mainWindowView);

            // Use this to create the main VM + finish
            mainWindowView.DataContext = container.GetInstance(typeof(MainWindowViewModel));
            singleViewPlatform.MainView = mainWindowView;

        }

        base.OnFrameworkInitializationCompleted();
    }
}