using ABCo.Multicam.UI.Avalonia.Views.Strips;
using ABCo.Multicam.UI.Avalonia.Views.Strips.Switcher;
using ABCo.Multicam.UI.Enumerations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia.Locators
{
    public class FeatureViewLocator : IValueConverter
    {
        public static readonly FeatureViewLocator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not FeatureViewType) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            FeatureViewType type = (FeatureViewType)value;
            return type switch
            {
                FeatureViewType.Switcher => new SwitcherStripView(),
                FeatureViewType.Unsupported => CreateUnsupportedView(),
                _ => new BindingNotification(new Exception("Unimplemented StripViewType value in the locator."), BindingErrorType.Error),
            };
        }

        Control CreateUnsupportedView()
        {
            var stripTitleControl = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
            stripTitleControl.Classes.Add("FeatureTitleText");
            stripTitleControl.Bind(TextBlock.TextProperty, new Binding("StripTitle"));

            var unsupportedText = new TextBlock { Text = "Unsupported - updating to the latest version may help.", Margin = new Thickness(20) };

            var stackPanelControl = new StackPanel() { Orientation = Orientation.Horizontal };
            stackPanelControl.Children.Add(stripTitleControl);
            stackPanelControl.Children.Add(unsupportedText);
            return stackPanelControl;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
