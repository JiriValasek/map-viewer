using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MapViewer.Wpf.Converters
{
    /// <summary>
    /// Inverse boolean convertor to visibility - true => Collapsed, false => Visible.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility && visibility == Visibility.Visible)
            {
                return false;
            }
            return true;
        }
    }
}
