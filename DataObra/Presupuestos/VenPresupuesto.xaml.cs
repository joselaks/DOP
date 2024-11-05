using Bibioteca.Clases;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private object _originalValue;
        public VenPresupuesto()
        {
            InitializeComponent();
            Objeto = new Presupuesto();
            Objeto.agregaNodo("R", null);
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaDetalle.ItemsSource = Objeto.Insumos;
            
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

            totMateriales.Value = Objeto.TotalMateriales;
            totMDO.Value = Objeto.TotalManodeObra;
            totEquipos.Value = Objeto.TotalEquipos;
            totSubcontratos.Value = Objeto.TotalSubcontratos;
            totOtros.Value= Objeto.TotalOtros;
            totGeneral.Value = Objeto.TotalDirecto;
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

        private void grillaArbol_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {


            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;

            if (column == "Cantidad")
            {
                var newValue = record.Cantidad.ToString();
                MessageBox.Show($"La cantidad cambió de {_originalValue} a {newValue}");
            }
            else if (column == "Descripcion")
            {
                var newValue = record.Descripcion;
                MessageBox.Show($"La descripción cambió de {_originalValue} a {newValue}");
            }

            // Restablece el valor original
            _originalValue = null;

        }


        private void grillaArbol_CurrentCellBeginEdit(object sender, TreeGridCurrentCellBeginEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;

            if (column == "Cantidad")
            {
                _originalValue = record.Cantidad;
            }
            else if (column == "Descripcion")
            {
                _originalValue = record.Descripcion;
            }
        }

        private void colCodigo_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "ID");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colTipo_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Tipo");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colMat_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Materiales");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void aRubro_Click(object sender, RoutedEventArgs e)
        {

        }

        private void colMDO_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "ManodeObra");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colEqi_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Equipos");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void colSub_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Subcontratos");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void colOtr_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Otros");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }
    }

}

