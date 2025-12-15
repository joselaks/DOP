using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DataObra.Interfaz.Clases
    {
    public class PrRealMayorAProyectadoConverter : IValueConverter
        {
        public Brush ColorMayor { get; set; } = Brushes.OrangeRed;
        public Brush ColorNormal { get; set; } = Brushes.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            // value es el objeto de fila (ConceptoConGastosPropio)
            if (value == null) return ColorNormal;

            dynamic row = value;
            try
                {
                decimal prReal = row.PrReal;
                decimal prEjec = row.PrEjec;
                if (prReal > prEjec)
                    return ColorMayor;
                }
            catch { }
            return ColorNormal;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }