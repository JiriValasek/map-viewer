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
    /// Convertor from a Vector3 to a Point3D or Vector3D.
    /// </summary>
    public class Vector3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector3 vector)
            {

                Func<double, double, double, object> itemConverter;

                if (targetType == typeof(Point3D))
                {
                    itemConverter = (double x, double y, double z) => new Point3D(x, y, z);
                }
                else if (targetType == typeof(Vector3D))
                {
                    itemConverter = (double x, double y, double z) => new Vector3D(x, y, z);
                }
                else
                {
                    throw new UnsupportedConversionException($"Target type {targetType} is not supported by Vector3Converter.");
                }

                return itemConverter(vector.X, vector.Y, vector.Z);
            }
            else
            {
                throw new UnsupportedConversionException($"Source object for Vector3CConverter must be a 'Vector3'.");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
