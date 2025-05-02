using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Numerics;
using System.Windows.Media.Media3D;
using MapViewer.Wpf.Exceptions;

namespace MapViewer.Wpf.Converters
{
    /// <summary>
    /// Convertor from a cursor vector to the status text.
    /// </summary>
    public class CursorToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector3 vector)
            {
                return String.Format("X: {0:0.00}, Y: {1:0.00}, Z: {2:0.00}", vector.X, vector.Y, vector.Z);
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
