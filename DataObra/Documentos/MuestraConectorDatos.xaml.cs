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

namespace DataObra.Documentos
{
    /// <summary>
    /// Lógica de interacción para MuestraConectorDatos.xaml
    /// </summary>
    public partial class MuestraConectorDatos : Window
    {
        public MuestraConectorDatos()
        {
            InitializeComponent();
            _ = obtenerDocsAsync();
        }


        private async Task obtenerDocsAsync()
        {
        }

    }
}
