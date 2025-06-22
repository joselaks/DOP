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
using Syncfusion.XlsIO;

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
        public UcDosaje Dosaje;
        public WiPresupuesto(int usuario, int presupuesto )
        {
            InitializeComponent();
            Objeto = new Presupuesto(null);
            Dosaje = new UcDosaje(Objeto);
            Planilla = new UcPlanilla(Objeto, Dosaje);
            Listado = new UcListado(Objeto);
            this.gPlanilla.Children.Add(Planilla);
            this.gListado.Children.Add(Listado);
            this.gDetalle.Children.Add(Dosaje);
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
            // Crear el presupuesto principal
            var presupuestoDto = new PresupuestoDTO
            {
                ID = 11,
                CuentaID = 1,
                UsuarioID = 2,
                Descrip = "Presupuesto de prueba modificado",
                PrEjecTotal = 1000,
                PrEjecDirecto = 800,
                EjecMoneda = 'P',
                PrVentaTotal = 1200,
                PrVentaDirecto = 900,
                VentaMoneda = 'P',
                Superficie = 150,
                MesBase = DateTime.Now,
                FechaC = DateTime.Now,
                FechaM = DateTime.Now,
                EsModelo = false,
                TipoCambioD = 1200
            };

            // Crear lista de conceptos (3 registros de ejemplo)
            var conceptos = new List<ConceptoDTO>
    {
        new ConceptoDTO
        {
            PresupuestoID = 0,
            ConceptoID = "C1",
            Descrip = "Concepto 1",
            Tipo = 'M',
            Unidad = "m2",
            PrEjec = 100,
            PrVent = 120,
            EjecMoneda = 'P',
            VentMoneda = 'P',
            MesBase = DateTime.Now,
            CanTotalEjec = 10,
            InsumoID = 1,
            Accion = 'A'
        },
        new ConceptoDTO
        {
            PresupuestoID = 0,
            ConceptoID = "C2",
            Descrip = "Concepto 2",
            Tipo = 'O',
            Unidad = "u",
            PrEjec = 200,
            PrVent = 220,
            EjecMoneda = 'P',
            VentMoneda = 'P',
            MesBase = DateTime.Now,
            CanTotalEjec = 5,
            InsumoID = 2,
            Accion = 'I'
        },
        new ConceptoDTO
        {
            PresupuestoID = 0,
            ConceptoID = "C3",
            Descrip = "Concepto 3",
            Tipo = 'E',
            Unidad = "kg",
            PrEjec = 300,
            PrVent = 330,
            EjecMoneda = 'P',
            VentMoneda = 'P',
            MesBase = DateTime.Now,
            CanTotalEjec = 20,
            InsumoID = 3,
            Accion = 'A'
        }
    };

            // Crear lista de relaciones (3 registros de ejemplo)
            var relaciones = new List<RelacionDTO>
    {
        new RelacionDTO
        {
            PresupuestoID = 0,
            CodSup = "C1",
            CodInf = "C2",
            CanEjec = 2,
            CanVenta = 2,
            OrdenInt = 1,
            Accion = 'A'
        },
        new RelacionDTO
        {
            PresupuestoID = 0,
            CodSup = "C1",
            CodInf = "C3",
            CanEjec = 3,
            CanVenta = 3,
            OrdenInt = 2,
            Accion = 'A'
        },
        new RelacionDTO
        {
            PresupuestoID = 0,
            CodSup = "C2",
            CodInf = "C3",
            CanEjec = 1,
            CanVenta = 1,
            OrdenInt = 3,
            Accion = 'A'
        }
    };

            // Armar el request
            var request = new DOP.Datos.ProcesaPresupuestoRequest
            {
                Presupuesto = presupuestoDto,
                Conceptos = conceptos,
                Relaciones = relaciones
            };

            // Llamar al servicio
            var (success, message, presupuestoID) = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(request);

            // Mostrar resultado
            if (success)
            {
                MessageBox.Show($"Presupuesto guardado correctamente. ID: {presupuestoID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Planilla.colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            //colImporte2.HeaderText = $"{totalGeneralDol.ToString("N2", cultura)}";


            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RibbonButton boton)
            {
                switch (boton.Name)
                {
                    case "Rubro":
                        // Lógica para agregar Rubro
                        var (nuevoNodo, mensaje) = Objeto.agregaNodo("R", null);
                        break;
                    case "Tarea":
                        // Lógica para agregar Tarea
                        if (this.Planilla.grillaArbol.SelectedItem == null)
                        {
                            MessageBox.Show("debe seleccionar un rubro para la tarea");
                        }
                        else
                        {
                            Bibioteca.Clases.Nodo sele = this.Planilla.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                            if (sele.Tipo != "R")
                            {
                                MessageBox.Show("debe seleccionar un rubro para la tarea");
                            }
                            else
                            {
                                Objeto.agregaNodo("T", sele);
                            }
                        }
                        break;
                    default:
                        // Otro caso
                        break;
                }
            }
        }

        private void Recalculo_Click(object sender, RoutedEventArgs e)
            {
             recalculo();
            }
        }
}
