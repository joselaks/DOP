using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DOP.Presupuestos.Clases
    {
    public class DecimalToStringConverter : IValueConverter
        {
        private static readonly CultureInfo cultura = new CultureInfo("es-ES");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            if (value is decimal dec)
                return dec.ToString("N2", cultura);
            if (value is double dbl)
                return dbl.ToString("N2", cultura);
            return value?.ToString();
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            if (value is string s && decimal.TryParse(s, NumberStyles.Any, cultura, out var result))
                return result;
            return value;
            }
        }
    }
