using MapViewer.Wpf.Exceptions;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MapViewer.Wpf.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
            {

                if (targetType == typeof(Brush))
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                }
                else if (targetType == typeof(System.Windows.Media.Color))
                {
                    return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                }
                else
                {
                    throw new UnsupportedConversionException($"Target type {targetType} is not supported by ColorConverter.");
                }
            }
            else
            {
                throw new UnsupportedConversionException($"Source object for ColorConverter must be a 'System.Drawing.Color'.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
