using System;
using System.Globalization;
using System.Windows.Data;

namespace DataObra.Presupuestos.Clases
    {
    public class ValorM2Converter : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            dynamic row = value;
            try
                {
                decimal total = row.PrEjecTotal;
                decimal superficie = row.Superficie;
                if (superficie > 0)
                    {
                    decimal resultado = total / superficie;
                    if (resultado == 0)
                        return "Sin Datos";
                    return resultado.ToString("N2");
                    }
                else
                    return "-";
                }
            catch
                {
                return "-";
                }
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }