using MapViewer.Wpf.Exceptions;
using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MapViewer.Wpf.Converters
{

    /// <summary>
    /// Convertor from a IEnumerable<Vector2> to a PointCollection.
    /// </summary>
    public class Vector2CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Vector2> vectors)
            {
                IList convertedCollection;
                Func<double, double, object> itemConverter;

                if (targetType == typeof(PointCollection))
                {
                    convertedCollection = new PointCollection();
                    itemConverter = (double x, double y) => new Point(x, y);
                }
                else
                {
                    throw new UnsupportedConversionException($"Target type {targetType} is not supported by Vector2CollectionConverter.");
                }

                foreach (var vector in vectors)
                {
                    convertedCollection.Add(itemConverter(vector.X, vector.Y));
                }
                return convertedCollection;

            }
            else
            {
                throw new UnsupportedConversionException($"Source object for Vector2CollectionConverter must be a 'IEnumerable<Vector2>'.");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
