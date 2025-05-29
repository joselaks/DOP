using Bibioteca.Clases;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Biblioteca;
using DOP.Presupuestos.Controles.SubControles;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DOP.Presupuestos.Controles
    {
    /// <summary>
    /// Lógica de interacción para UcPresupuesto.xaml
    /// </summary>
    public partial class UcPresupuesto : UserControl
        {
        private SidePanel rightPanel;
        public Presupuesto Objeto;
        

        public UcPresupuesto()
            {
            //SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "DockingManager", "TabControlExt" }));

            InitializeComponent();
            Objeto = new Presupuesto(null);
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            UcListaConceptos listaInsumos = new UcListaConceptos(Objeto);
            this.listado.Children.Add(listaInsumos);
            this.grillaArbol.Loaded += GrillaArbol_Loaded;
        }

        private void GrillaArbol_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.View != null)
            {
                this.grillaArbol.View.Filter = FiltrarPorTipo;
                this.grillaArbol.View.Refresh();
            }
            var panels = Syncfusion.Windows.Shared.VisualUtils.EnumChildrenOfType(docPres, typeof(SidePanel));
            if (panels != null)
                {
                foreach (SidePanel panel in panels)
                    {
                    if (panel.Name.ToString() == "PART_RightPanel")
                        {
                        rightPanel = panel;
                        rightPanel.FontSize = 20;
                        rightPanel.FontWeight = FontWeights.DemiBold;
                        rightPanel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        rightPanel.LayoutUpdated += Panel_LayoutUpdated;
                        }
                    }
                }
            }

        private void Panel_LayoutUpdated(object sender, EventArgs e)
            {
            DirectTabPanel tabpanel = rightPanel.Template.FindName("PART_PanelName", rightPanel) as DirectTabPanel;
            if (tabpanel != null)
                {
                tabpanel.VerticalAlignment = VerticalAlignment.Center;
                rightPanel.LayoutUpdated -= Panel_LayoutUpdated;
                }
            }

        private bool FiltrarPorTipo(object item)
        {
            if (item is Nodo nodo)
            {
                return nodo.Tipo == "R" || nodo.Tipo == "T";
            }
            return false;
        }


        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {

        }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {

        }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
        {

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
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            colImporte2.HeaderText = $"{totalGeneralDol.ToString("N2", cultura)}";


            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }
    }
    }
