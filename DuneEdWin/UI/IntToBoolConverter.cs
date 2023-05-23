using System;
using System.Globalization;
using System.Windows.Data;

namespace DuneEdWin.UI
{
    internal class IntToBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            if (int.TryParse(value.ToString(), out int intval))
            {
                return intval != 0;
            }
            return false;
        } // Convert

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // ConvertBack
    } // class IntToVisibilityConverter
} // namespace
