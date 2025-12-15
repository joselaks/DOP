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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            // Si el valor es null, retorna el color por defecto (por ejemplo, negro)
            if (value == null)
                return Brushes.Black;

            // Si el valor no es decimal, intenta convertirlo
            decimal numero;
            if (value is decimal d)
                numero = d;
            else if (!decimal.TryParse(value.ToString(), out numero))
                return Brushes.Black;

            // Lógica original: si es negativo, rojo; si no, negro
            return numero < 0 ? Brushes.Red : Brushes.Black;
            }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }
