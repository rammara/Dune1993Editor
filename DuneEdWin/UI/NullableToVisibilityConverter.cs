using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DuneEdWin.UI
{
    internal class NullableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || false == (bool)value) return Visibility.Collapsed;
            return Visibility.Visible;
        } // Convert

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // ConvertBack
    }
}
