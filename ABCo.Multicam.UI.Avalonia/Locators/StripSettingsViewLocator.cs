using ABCo.Multicam.UI.Avalonia.Views.Strips.Switcher;
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
    public class StripSettingsViewLocator : IValueConverter
    {
        public static readonly StripSettingsViewLocator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not StripViewType) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            StripViewType type = (StripViewType)value;
            return type switch
            {
                StripViewType.Switcher => new SwitcherStripSettingsView(),
                StripViewType.Unsupported => new TextBlock { Text = "Unsupported strip." },
                _ => new BindingNotification(new Exception("Unimplemented StripViewType value in the locator."), BindingErrorType.Error),
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
