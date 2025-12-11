using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DataObra.Interfaz.Clases
{
    public class NumerosRojos : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            decimal input = (decimal)value;

            if (input < 0)
                return new SolidColorBrush(Colors.Red);

            else
                return DependencyProperty.UnsetValue;
            }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }
