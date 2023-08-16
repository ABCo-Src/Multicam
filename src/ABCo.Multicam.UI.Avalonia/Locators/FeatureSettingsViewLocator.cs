using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Avalonia.Views.Features.Switcher;
using ABCo.Multicam.UI.Enumerations;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia.Locators
{
    public class FeatureSettingsViewLocator : IValueConverter
    {
        public static readonly FeatureSettingsViewLocator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not FeatureTypes) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            FeatureTypes type = (FeatureTypes)value;
            return type switch
            {
                FeatureTypes.Switcher => new SwitcherFeatureSettingsView(),
                FeatureTypes.Unsupported => new TextBlock { Text = "Unsupported feature." },
                _ => new BindingNotification(new Exception("Unimplemented FeatureViewType value in the locator."), BindingErrorType.Error),
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
