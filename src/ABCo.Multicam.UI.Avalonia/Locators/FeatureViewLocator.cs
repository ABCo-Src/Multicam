using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Avalonia.Views.Features.Switcher;
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
            if (value is not FeatureTypes) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            FeatureTypes type = (FeatureTypes)value;
            return type switch
            {
                FeatureTypes.Switcher => new SwitcherFeatureView(),
                FeatureTypes.Unsupported => CreateUnsupportedView(),
                _ => new BindingNotification(new Exception("Unimplemented FeatureTypes value in the locator."), BindingErrorType.Error),
            };
        }

        Control CreateUnsupportedView()
        {
            var featureTitleControl = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
            featureTitleControl.Classes.Add("FeatureTitleText");
            featureTitleControl.Bind(TextBlock.TextProperty, new Binding("FeatureTitle"));

            var unsupportedText = new TextBlock { Text = "Unsupported - updating to the latest version may help.", Margin = new Thickness(20) };

            var stackPanelControl = new StackPanel() { Orientation = Orientation.Horizontal };
            stackPanelControl.Children.Add(featureTitleControl);
            stackPanelControl.Children.Add(unsupportedText);
            return stackPanelControl;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
