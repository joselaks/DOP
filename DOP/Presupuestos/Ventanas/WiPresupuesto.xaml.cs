using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DOP.Presupuestos.Controles;
using Bibioteca.Clases;
using DOP.Presupuestos.Clases;
using Microsoft.Win32;
using System.IO;
using System.Globalization;

namespace DOP.Presupuestos.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiPresupuesto.xaml
    /// </summary>
    public partial class WiPresupuesto : RibbonWindow
    {
        public Presupuesto Objeto;
        public cUndoRedo UndoRedo;
        public UcPlanilla Planilla;
        public UcListado Listado;
        public WiPresupuesto()
        {
            InitializeComponent();
            Objeto = new Presupuesto(null);
            this.gPlanilla.Children.Add(Planilla = new UcPlanilla(Objeto));
            this.gListado.Children.Add(Listado = new UcListado(Objeto));

        }

        private void Fiebdc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFiebdc_Click(object sender, RoutedEventArgs e)
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
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto(null);

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto(null);
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

            //totMateriales1.Value = Objeto.Arbol.Sum(i => i.Materiales1);
            //totMDO1.Value = Objeto.Arbol.Sum(i => i.ManodeObra1);
            //totEquipos1.Value = Objeto.Arbol.Sum(i => i.Equipos1);
            //totSubcontratos1.Value = Objeto.Arbol.Sum(i => i.Subcontratos1);
            //totOtros1.Value = Objeto.Arbol.Sum(i => i.Otros1);
            //totGeneral1.Value = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totGeneral1 = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totalGeneralDol = Objeto.Arbol.Sum(i => i.Importe2);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            //colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            //colImporte2.HeaderText = $"{totalGeneralDol.ToString("N2", cultura)}";


            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }
    }
}
