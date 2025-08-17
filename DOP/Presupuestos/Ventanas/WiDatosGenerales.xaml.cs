using Biblioteca.DTO;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiDatosGenerales.xaml
    /// </summary>
    public partial class WiDatosGenerales : Window
        {
        public WiDatosGenerales(PresupuestoDTO encabezado)
            {
            InitializeComponent();
            // Clona los datos para edición temporal si lo deseas, aquí se enlaza directamente
            this.DataContext = encabezado;
            }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
            {
            this.DialogResult = true;
            }
        }
    }
