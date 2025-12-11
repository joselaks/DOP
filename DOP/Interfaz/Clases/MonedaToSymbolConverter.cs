using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataObra.Interfaz.Clases
    {
    public class MonedaToSymbolConverter : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            if (value == null) return string.Empty;

            // Acepta char o string
            var s = value is char c ? c.ToString() : value.ToString();

            if (string.Equals(s, "P", StringComparison.OrdinalIgnoreCase))
                return "$";
            if (string.Equals(s, "D", StringComparison.OrdinalIgnoreCase))
                return "u$s";

            return s;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            // No necesario en este escenario
            throw new NotSupportedException();
            }
        }
    }