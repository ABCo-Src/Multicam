using ABCo.Multicam.UI.Enumerations;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia.Converters
{
    public class SwitcherStripStatusToColor : IValueConverter
    {
        public static readonly SwitcherStripStatusToColor Instance = new();

        // Dark:
        static readonly SolidColorBrush _neutralInactive = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
        static readonly SolidColorBrush _neutralActive = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        static readonly SolidColorBrush _previewActive = new SolidColorBrush(Color.FromRgb(0x33, 0x80, 0x33));
        static readonly SolidColorBrush _programActive = new SolidColorBrush(Color.FromRgb(0x80, 0x33, 0x33));

        // Light:
        //static readonly SolidColorBrush _neutralInactive = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
        //static readonly SolidColorBrush _neutralActive = new SolidColorBrush(Color.FromRgb(0xaa, 0xaa, 0xaa));
        //static readonly SolidColorBrush _previewActive = new SolidColorBrush(Color.FromRgb(0x66, 0xff, 0x66));
        //static readonly SolidColorBrush _programActive = new SolidColorBrush(Color.FromRgb(0xff, 0x66, 0x66));

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not SwitcherButtonStatus || targetType != typeof(IBrush)) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            var status = (SwitcherButtonStatus)value;
            return status switch
            {
                SwitcherButtonStatus.NeutralInactive => _neutralInactive,
                SwitcherButtonStatus.PreviewInactive => _neutralInactive,
                SwitcherButtonStatus.ProgramInactive => _neutralInactive,
                SwitcherButtonStatus.NeutralActive => _neutralActive,
                SwitcherButtonStatus.PreviewActive => _previewActive,
                SwitcherButtonStatus.ProgramActive => _programActive,
                _ => throw new Exception("Unrecognised switcher button status in color converter.")
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
