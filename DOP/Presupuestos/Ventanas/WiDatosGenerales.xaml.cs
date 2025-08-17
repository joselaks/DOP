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

            // Si MesBase es 01/01/0001, poner la fecha actual
            if (encabezado.MesBase == DateTime.MinValue)
                {
                encabezado.MesBase = DateTime.Today;
                }

            this.DataContext = encabezado;
            }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
            {
            this.DialogResult = true;
            }
        }
    }
