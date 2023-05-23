using System;
using System.Globalization;
using System.Windows.Data;

namespace DuneEdWin.UI
{
    internal class GameInfoWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0;
            return UIConstants.GameInfoWidth;
        } // Convert

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // ConvertBack
    } // class GameInfoWidthConverter
} // namespace
