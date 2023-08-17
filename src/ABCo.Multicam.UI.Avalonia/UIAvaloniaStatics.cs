using ABCo.Multicam.UI.Avalonia.Services;
using ABCo.Multicam.UI.Avalonia.Views;
using ABCo.Multicam.UI.Services;
using Avalonia.Controls;
using LightInject;

namespace ABCo.Multicam.UI.Avalonia
{
    public static class UIAvaloniaStatics
    {
        public static ServiceContainer InitializeContainer(Window? mainWindow, MainWindowView mainView)
        {
            var container = new ServiceContainer(new ContainerOptions { EnablePropertyInjection = false });

            // Register general services
            container.RegisterInstance<IUIDialogHandler>(new UIDialogHandler(mainView));

            // Register platform-specific services
            if (mainWindow == null)
                container.RegisterSingleton<IUIWindow, UnwindowedUIWindow>();
            else
                container.RegisterInstance<IUIWindow>(new WindowedUIWindow(mainWindow!));

            // Register the next layer down now
            UIStatics.Initialize(container);

            return container;
        }
    }
}
