using System;
using System.Globalization;
using System.Windows.Data;

namespace DataObra.Presupuestos.Ventanas
    {
    public class UnidadGlOcultaConverter : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            var data = value as dynamic;
            if (data == null) return value;

            string unidad = "";
            try
                {
                unidad = (string)data.Unidad;
                }
            catch { }

            if (!string.IsNullOrEmpty(unidad) && unidad.Equals("gl", StringComparison.OrdinalIgnoreCase))
                return null;

            if (parameter != null && data != null)
                {
                var prop = data.GetType().GetProperty(parameter.ToString());
                if (prop != null)
                    {
                    var val = prop.GetValue(data, null);
                    if (val is double || val is float || val is decimal)
                        return string.Format(new CultureInfo("es-ES"), "{0:N2}", val);
                    return val;
                    }
                }
            return value;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            return value;
            }
        }
    }