using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Planificacion
    {
    /// <summary>
    /// Lógica de interacción para UcPlanificacion.xaml
    /// </summary>
    public partial class UcPlanificacion : UserControl
        {
        public UcPlanificacion()
            {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            }
        }
    }
