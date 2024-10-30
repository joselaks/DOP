using Bibioteca.Clases;
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

namespace DataObra.Presupuestos
{
    /// <summary>
    /// Lógica de interacción para VenPresupuesto.xaml
    /// </summary>
    public partial class VenPresupuesto : Window
    {
        public VenPresupuesto()
        {
            InitializeComponent();
            Presupuesto Objeto = new Presupuesto();
            Objeto.agregaNodo("R", null);
            this.grillaArbol.ItemsSource = Objeto.Arbol;
        }

    }
}
