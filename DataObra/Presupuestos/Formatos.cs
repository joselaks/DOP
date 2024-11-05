using Bibioteca.Clases;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DataObra.Presupuestos
{
    
        public class BackgroundConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var data = value as Nodo;
                if (data != null && data.Tipo == "R")
                {
                return Brushes.Red;
                }
                return Brushes.Transparent;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    

}
