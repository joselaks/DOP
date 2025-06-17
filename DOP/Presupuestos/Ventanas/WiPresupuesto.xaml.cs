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
using Biblioteca.DTO;
using Syncfusion.Windows.Shared;

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
        public WiPresupuesto(int usuario, int presupuesto )
        {
            InitializeComponent();
            Objeto = new Presupuesto(null);
            this.gPlanilla.Children.Add(Planilla = new UcPlanilla(Objeto));
            this.gListado.Children.Add(Listado = new UcListado(Objeto));
            this.Closing += WiPresupuesto_Closing; // Suscribir el evento

            }

        private void WiPresupuesto_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
            {
            // Ejemplo: Confirmar cierre
            var result = MessageBox.Show("¿Está seguro que desea cerrar el presupuesto?", "Confirmar cierre", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                {
                e.Cancel = true; // Cancela el cierre
                }

            // Aquí puedes agregar lógica adicional, como guardar cambios, liberar recursos, etc.
            }


        private void Fiebdc_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {

            //Ojo....por ahora funciona solo para nuevos 
            // Validar que haya datos mínimos
            if (Objeto == null || Objeto.encabezado == null)
                {
                MessageBox.Show("No hay datos de presupuesto para guardar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            // Armar el request
            Objeto.listaConceptosGrabar.Clear();
            Objeto.listaRelacionesGrabar.Clear();
            Objeto.aplanar(Objeto.Arbol, null);
            var request = new DOP.Datos.ProcesaPresupuestoRequest
                {
                Presupuesto = new Biblioteca.DTO.PresupuestoDTO
                    {
                    ID = 0, // Nuevo presupuesto
                    CuentaID = null,
                    UsuarioID = 2,
                    Descrip = "Sin descripción",
                    PrEjecTotal = 100,
                    PrEjecDirecto = 200,
                    EjecMoneda = 'P',
                    PrVentaTotal = 300,
                    PrVentaDirecto = 400,
                    VentaMoneda = 'P',
                    Superficie = 500,
                    MesBase = DateTime.Now,
                    FechaC = DateTime.Now,
                    FechaM = DateTime.Now,
                    EsModelo = false,
                    TipoCambioD = 1200
                    },

                Conceptos = Objeto.listaConceptosGrabar ?? new List<ConceptoDTO>(),
                Relaciones = Objeto.listaRelacionesGrabar ?? new List<RelacionDTO>()

                };

            // Llamar al servicio
            var (success, message, presupuestoID) = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(request);

            // Mostrar resultado
            if (success)
                {
                MessageBox.Show($"Presupuesto guardado correctamente. ID: {presupuestoID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                // Puedes actualizar el estado local aquí si lo necesitas
                }
            else
                {
                MessageBox.Show($"Error al guardar el presupuesto:\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
