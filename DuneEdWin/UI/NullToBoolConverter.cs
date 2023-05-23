using System;
using System.Globalization;
using System.Windows.Data;

namespace DuneEdWin.UI
{
    internal class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is not null);
        } // Convert

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // ConvertBack
    } // NullToBoolConverter
} // namespace
