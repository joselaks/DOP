using Bibioteca.Clases;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataObra.Presupuestos
{
    /// <summary>
    /// Lógica de interacción para VenPresupuesto.xaml
    /// </summary>
    public partial class VenPresupuesto : Window
    {
        public Presupuesto Objeto;
        public VenPresupuesto()
        {
            InitializeComponent();
            Objeto = new Presupuesto();
            Objeto.agregaNodo("R", null);
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
           
        }

        

        private void Fiebdc_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Archivo Fiebdc|*.bc3";

            if (openFileDialog.ShowDialog().Value)
            {
                FileStream stream = File.OpenRead(openFileDialog.FileName);
                string textoFie;
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    textoFie = reader.ReadToEnd();
                    //this.txtArchivoActualiza.Text = "Archivo seleccionado";
                    string txtNombre = stream.Name;
                }
                Bibioteca.Clases.Fiebdc fie = new Bibioteca.Clases.Fiebdc(textoFie);
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto();

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto();
                objetofieb.generaPresupuesto("fie", fie.listaConceptos, fie.listaRelaciones);
                foreach (var item in objetofieb.Arbol)
                {
                    Objeto.Arbol.Add(item);
                }
                recalculo();
            }
        }
        public void recalculo()
        {
            Objeto.recalculo(Objeto.Arbol, true, 0, true);

            Objeto.sinCero();
            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }

        private void aTarea_Click(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar un rubro para la tarea");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "R")
                {
                    MessageBox.Show("debe seleccionar un rubro para la tarea");
                }
                else
                {
                    Objeto.agregaNodo("T", sele);
                }
            }
        }

        
    }
}
