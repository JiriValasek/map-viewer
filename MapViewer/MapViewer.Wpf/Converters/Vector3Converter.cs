using MapViewer.Wpf.Exceptions;
using System.Globalization;
using System.Numerics;
using System.Windows.Data;
using System.Windows.Media.Media3D;

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
