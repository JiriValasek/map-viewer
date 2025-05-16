using MapViewer.Core.Models;
using System.Globalization;
using System.Windows.Data;

namespace MapViewer.Wpf.Converters
{

    /// <summary>
    /// Convertor from a Circle to a status text
    /// </summary>
    public class CircleToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Circle circle)
            {
                return String.Format("X: {0:0.00}, Y: {1:0.00}, R: {2:0.00}", circle.Center.X, circle.Center.Y, circle.Radius);
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
