using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcDosaje.xaml
    /// </summary>
    public partial class UcDosaje : UserControl
    {
        public Presupuesto Objeto;
        public UcDosaje(Presupuesto objeto) 
        {
            InitializeComponent();
            //this.grillaDetalle.ChildPropertyName = "Inferiores";
            Objeto = objeto;
            //grillaDetalle.ItemsSource = Objeto.Arbol;


        }


        public void MostrarInferiores(ObservableCollection<Nodo> inferiores)
        {
            grillaDetalle.ItemsSource = inferiores;

        }

    }
}
