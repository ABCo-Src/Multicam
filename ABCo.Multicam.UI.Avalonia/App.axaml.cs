using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.Avalonia.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ABCo.Multicam.UI.Avalonia.Controls.Window;

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

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var container = UIAvaloniaStatics.InitializeContainer(desktop);
            desktop.MainWindow = new MainWindow()
            {
                DataContext = container.GetInstance(typeof(MainWindowViewModel))
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var container = UIAvaloniaStatics.InitializeContainer(singleViewPlatform);
            singleViewPlatform.MainView = new MainWindowView()
            {
                DataContext = container.GetInstance(typeof(MainWindowViewModel))
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}