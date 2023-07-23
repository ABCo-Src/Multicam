using ABCo.Multicam.UI.Avalonia.Controls.Window;
using ABCo.Multicam.UI.Avalonia.Views;
using ABCo.Multicam.UI.Services;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia.Services
{
    public class UIDialogHandler : IUIDialogHandler
    {
        MainWindowView _mainView;
        public UIDialogHandler(MainWindowView window) => _mainView = window;

        public void OpenSimpleContext<T>(string title, Action<T> selected, Action? cancel, ContextMenuItem<T>[] items)
        {
            // Create items
            var itemsControl = new StackPanel();

            if (title != "")
            {
                var titleControl = new TextBlock() { Text = title };
                titleControl.Classes.Add("ContextMenuTitle");
                itemsControl.Children.Add(titleControl);
            }

            for (int i = 0; i < items.Length; i++)
            {
                var button = new Button() { Content = items[i].Name };
                button.Classes.Add("Borderless");
                button.Classes.Add("ContextMenuButton");

                // TODO: Optimize click event here
                var itemCapture = items[i].Value;
                button.Click += (s, e) => selected(itemCapture);

                itemsControl.Children.Add(button);
            }

            // Set flyout
            SetFlyout(itemsControl, cancel);
        }

        private void SetFlyout(Control content, Action? cancel)
        {
            var control = new Flyout()
            {
                Placement = PlacementMode.Pointer,
                Content = content
            };

            FlyoutBase.SetAttachedFlyout(_mainView, control);
            control.ShowAt(_mainView, true);

            if (cancel != null)
                control.Closed += (s, e) => cancel();
        }
    }
}
