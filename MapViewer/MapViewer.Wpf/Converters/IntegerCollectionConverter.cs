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
    /// Convertor from a IEnumerable<int> to a Int32Collection.
    /// </summary>
    public class IntegerCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<int> integers)
            {
                IList convertedCollection;

                if (targetType == typeof(Int32Collection))
                {
                    convertedCollection = new Int32Collection();
                }
                else
                {
                    throw new UnsupportedConversionException($"Target type {targetType} is not supported by IntegerCollectionConverter.");
                }

                foreach (var ind in integers)
                {
                    convertedCollection.Add(ind);
                }
                return convertedCollection;

            }
            else
            {
                throw new UnsupportedConversionException($"Source object for IntegerCollectionConverter must be an 'IEnumerable<int>'.");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
