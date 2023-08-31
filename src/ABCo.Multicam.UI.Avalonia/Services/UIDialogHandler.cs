using ABCo.Multicam.UI.Avalonia.Views;
using ABCo.Multicam.UI.Services;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;

namespace ABCo.Multicam.UI.Avalonia.Services
{
	public class UIDialogHandler : IUIDialogHandler
    {
        readonly MainWindowView _mainView;
        public UIDialogHandler(MainWindowView window) => _mainView = window;

        //public void OpenContextMenu<T>(ContextMenuDetails<T> details)
        //{
        //    var itemsControl = new StackPanel();
        //    var menuInfo = new ContextMenuInfo<T>(CreateFlyout(itemsControl), details.OnSelect, details.OnCancel);

        //    // Create title
        //    if (details.Title != "")
        //    {
        //        var titleControl = new TextBlock() { Text = details.Title };
        //        titleControl.Classes.Add("ContextMenuTitle");
        //        itemsControl.Children.Add(titleControl);
        //    }

        //    // Create items
        //    for (int i = 0; i < details.Items.Length; i++)
        //    {
        //        var button = new Button() { Content = details.Items[i].Name };
        //        button.Classes.Add("Borderless");
        //        button.Classes.Add("ContextMenuButton");
        //        button.Tag = details.Items[i].Value;
        //        button.Click += menuInfo.HandleButtonClick;

        //        itemsControl.Children.Add(button);
        //    }

        //    // Show flyout
        //    ShowFlyout(menuInfo.Flyout);
        //}

		public void OpenContextMenu(ContextMenuDetails details)
		{
			throw new NotImplementedException();
		}

		private Flyout CreateFlyout(Control content)
        {
            var control = new Flyout()
            {
                Placement = PlacementMode.Pointer,
                Content = content
            };

            FlyoutBase.SetAttachedFlyout(_mainView, control);
            return control;
        }

        void ShowFlyout(Flyout flyout) => flyout.ShowAt(_mainView, true);

        class ContextMenuInfo<T>
        {
            public Flyout Flyout { get; }

            readonly Action<T> _select;
            Action? _cancel;

            public ContextMenuInfo(Flyout flyout, Action<T> select, Action? cancel)
            {
                (Flyout, _select, _cancel) = (flyout, select, cancel);

                if (_cancel != null)
                    flyout.Closed += HandleCancel;
            }

            public void HandleButtonClick(object? sender, RoutedEventArgs args)
            {
                var button = (Button)sender!;
                var value = (T)button.Tag!;

                // Run the action
                _select(value);

                // Disable cancel and close
                _cancel = null;
                Flyout.Hide();
            }

            public void HandleCancel(object? sender, EventArgs args) => _cancel?.Invoke();
        }
    }

}
