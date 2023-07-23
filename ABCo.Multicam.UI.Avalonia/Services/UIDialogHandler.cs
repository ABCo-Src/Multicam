using ABCo.Multicam.UI.Avalonia.Controls.Window;
using ABCo.Multicam.UI.Avalonia.Views;
using ABCo.Multicam.UI.Services;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

        public void OpenSimpleContext<T>(ContextMenuItem<T>[] items, Action<T> selected, Action? cancel)
        {
            // Create items
            var itemsCollection = new ItemsControl();

            for (int i = 0; i < items.Length; i++)
            {
                var button = new Button() { Content = items[i].Name };
                //button.Classes.Add("Borderless");
                button.Classes.Add("ContextMenuButton");
                itemsCollection.Items.Add(button);
            }

            // Set flyout
            SetFlyout(itemsCollection);
        }

        private void SetFlyout(Control content)
        {
            var control = new Flyout()
            {
                Placement = PlacementMode.Pointer,
                Content = content
            };

            FlyoutBase.SetAttachedFlyout(_mainView, control);
            control.ShowAt(_mainView, true);
        }
    }
}
