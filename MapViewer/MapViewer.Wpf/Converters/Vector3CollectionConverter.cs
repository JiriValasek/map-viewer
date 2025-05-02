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
using System.Collections;
using MapViewer.Wpf.Exceptions;
using System.Diagnostics;
using System.Windows.Media;

namespace MapViewer.Wpf.Converters
{

    /// <summary>
    /// Convertor from a IEnumerable<Vector3> to a Point3DCollection or Vector3DCollection.
    /// </summary>
    public class Vector3CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Vector3> vectors)
            {
                IList convertedCollection;
                Func<double, double, double, object> itemConverter;

                if (targetType == typeof(Point3DCollection))
                {
                    convertedCollection = new Point3DCollection();
                    itemConverter = (double x, double y, double z) => new Point3D(x, y, z);
                }
                else if (targetType == typeof(Vector3DCollection))
                {
                    convertedCollection = new Vector3DCollection();
                    itemConverter = (double x, double y, double z) => new Vector3D(x, y, z);
                }
                else
                {
                    throw new UnsupportedConversionException($"Target type {targetType} is not supported by Vector3CollectionConverter.");
                }
                foreach (var vector in vectors)
                {
                    convertedCollection.Add(itemConverter(vector.X, vector.Y, vector.Z));
                }
                return convertedCollection;

            }
            else
            {
                throw new UnsupportedConversionException($"Source object for Vector3CollectionConverter must be a 'IEnumerable<Vector3>'.");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
