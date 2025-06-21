using Bibioteca.Clases;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
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

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcPlanilla.xaml
    /// </summary>
    public partial class UcPlanilla : UserControl
    {
        public Presupuesto Objeto;
        public UcDosaje Dosaje;
        Nodo anterior = new Nodo();
        private object _originalValue;
        private Stack<Cambios> undoStack;
        private Stack<Cambios> redoStack;


        public UcPlanilla(Presupuesto objeto, UcDosaje dosaje)
        {
            InitializeComponent();
            Dosaje = dosaje;
            Objeto = objeto;
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaArbol.Loaded += GrillaArbol_Loaded;

        }

        // Le hemos agregado un evento Loaded al TreeGrid para aplicar el filtro una vez que la vista se haya cargado.
        private void GrillaArbol_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.View != null)
            {
                this.grillaArbol.View.Filter = FiltrarPorTipo;
                this.grillaArbol.View.Refresh();
            }
        }
        // Método para filtrar los nodos que se mostrarán en el TreeGrid.
        private bool FiltrarPorTipo(object item)
        {
            if (item is Nodo nodo)
            {
                return nodo.Tipo == "R" || nodo.Tipo == "T";
            }
            return false;
        }
        //Cada vez que se edita la grilla, se clona el objeto original antes de editarlo.
        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            // Clonar el objeto record y asignarlo a anterior
            anterior = Objeto.clonar(record);

            if (column == "ID")
            {
                _originalValue = record.ID;
            }

        }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            edicion(editado, column);
            var undoRegistro = new Cambios
            {
                TipoCambio = "Tipeo",
                antesCambio = anterior,
                despuesCambio = Objeto.clonar(editado),
                PropiedadCambiada = column,
                OldValue = _originalValue,
                NewValue = editado.GetType().GetProperty(column).GetValue(editado)
            };

        }

        private void edicion(Nodo? editado, string column)
        {
            switch (column)
            {
                case "ID":
                    Objeto.cambiaCodigo(Objeto.Arbol, editado.ID, _originalValue.ToString());
                    break;
                case "Cantidad":
                    CambioAuxiliar dato = new CambioAuxiliar();
                    dato.IdInferior = editado.ID;
                    dato.IdSuperior = Objeto.FindParentNode(Objeto.Arbol, editado, null).ID;
                    dato.Cantidad = editado.Cantidad;
                    Objeto.cambioCantidadAuxiliar(Objeto.Arbol, dato);
                    break;
                default:
                    Objeto.mismoCodigo(Objeto.Arbol, editado);
                    break;
            }
            recalculo();
        }

        public void recalculo()
        {
            Objeto.recalculo(Objeto.Arbol, true, 0, true);

            Objeto.sinCero();

            //totMateriales1.Value = Objeto.Arbol.Sum(i => i.Materiales1);
            //totMDO1.Value = Objeto.Arbol.Sum(i => i.ManodeObra1);
            //totEquipos1.Value = Objeto.Arbol.Sum(i => i.Equipos1);
            //totSubcontratos1.Value = Objeto.Arbol.Sum(i => i.Subcontratos1);
            //totOtros1.Value = Objeto.Arbol.Sum(i => i.Otros1);
            //totGeneral1.Value = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totGeneral1 = Objeto.Arbol.Sum(i => i.Importe1);
            //decimal totalGeneralDol = Objeto.Arbol.Sum(i => i.Importe2);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";


            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void grillaArbol_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (grillaArbol.SelectedItem is Nodo nodoSeleccionado && Dosaje != null && nodoSeleccionado.Tipo=="T")
            {
                Dosaje.MostrarInferiores(nodoSeleccionado);
            }

        }
    }
    public class Cambios
    {
        public string TipoCambio { get; set; }
        public Nodo antesCambio { get; set; }
        public Nodo despuesCambio { get; set; }
        public string PropiedadCambiada { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public Nodo NodoPadre { get; set; }
        public Nodo NodoMovido { get; set; }
        public Nodo NodoPadreNuevo { get; set; }
        public Nodo NodoPadreAnterior { get; set; }
        public int Posicion { get; set; }
    }
}
