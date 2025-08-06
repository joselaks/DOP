using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DOP.Datos;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
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
using System.Windows.Shapes;

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
        public UcMaestro Maestro;
        public UcArticulos Articulos;
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;


        public WiPresupuesto(PresupuestoDTO? _encabezado, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, ObservableCollection<PresupuestoDTO> presupuestosRef)
            {
            InitializeComponent();
            Objeto = new Presupuesto(_encabezado, conceptos, relaciones);

            Objeto.encabezado.UsuarioID = App.IdUsuario;
            Dosaje = new UcDosaje(Objeto);
            Planilla = new UcPlanilla(Objeto, Dosaje);
            Listado = new UcListado(Objeto);
            this.gDetalle.Children.Add(Dosaje);
            Maestro = new UcMaestro();
            Articulos = new UcArticulos();  
            this.gPlanilla.Children.Add(Planilla);
            //this.gPrecios.Children.Add(Articulos);
            //this.docTareas.Content = Tareas;
            this.Closing += WiPresupuesto_Closing; // Suscribir el evento
            _presupuestosRef = presupuestosRef;
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            //if (screenWidth > 1920 && screenHeight > 1080)


            if (screenWidth >= 1920 && screenHeight >= 1080)
                {
                // Resolución mayor: tamaño fijo y centrado
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.WindowState = WindowState.Normal;
                this.Width = 1900;
                this.Height = 1000;
                }
            else
                {
                // Resolución igual o menor: maximizar
                this.WindowState = WindowState.Maximized;
                }


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
            ProcesaPresupuestoRequest oGrabar = Objeto.EmpaquetarPresupuesto();

            // Llamada al servicio web para procesar el presupuesto
            var resultado = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(oGrabar);

            if (resultado.Success)
                {
                // --- ACTUALIZACIÓN DE LISTAS PARA PRÓXIMA EJECUCIÓN ---
                Objeto.listaConceptosLeer = Objeto.listaConceptosGrabar.Select(x => x).ToList();
                Objeto.listaRelacionesLeer = Objeto.listaRelacionesGrabar.Select(x => x).ToList();

                MessageBox.Show($"Presupuesto guardado correctamente. ID: {resultado.PresupuestoID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Asignar siempre la fecha de modificación actual
                Objeto.encabezado.FechaM = DateTime.Now;

                if (Objeto.encabezado.ID == null)
                    {
                    // Nuevo presupuesto: asignar fecha de creación y ID, luego agregar a la colección
                    Objeto.encabezado.FechaC = DateTime.Today;
                    Objeto.encabezado.ID = resultado.PresupuestoID;
                    _presupuestosRef.Add(Objeto.encabezado);
                    }
                else
                    {
                    // Edición: buscar y reemplazar en la colección
                    var existente = _presupuestosRef.FirstOrDefault(p => p.ID == Objeto.encabezado.ID);
                    if (existente != null)
                        {
                        var idx = _presupuestosRef.IndexOf(existente);
                        _presupuestosRef[idx] = Objeto.encabezado;
                        }
                    else
                        {
                        // Si no se encuentra, lo agrega (caso raro)
                        _presupuestosRef.Add(Objeto.encabezado);
                        }
                    }
                }
            else
                {
                MessageBox.Show($"Error al guardar el presupuesto: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto(null, null, null);

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto(null, null, null);
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
            Objeto.Rubros.Clear();
            Objeto.Tareas.Clear();
            Objeto.Auxiliares.Clear();
            Objeto.listaRubrosTareasAuxiliares(Objeto.Arbol);

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

        private void Recnumerar_Click(object sender, RoutedEventArgs e)
            {
            Objeto.NumeraItems(Objeto.Arbol, "");
            }

        private async void OInsumo_Click(object sender, RoutedEventArgs e)
            {
            var (success1, message1, insumos) = await DatosWeb.ObtenerInsumosPorUsuarioAsync(2);
            if (success1)
                {
                foreach (var insumo in insumos)
                    MessageBox.Show($"{insumo.ID} - {insumo.Descrip}");
                }
            else
                {
                MessageBox.Show($"Error: {message1}");
                }
            }

        private void SaleExcel_Click(object sender, RoutedEventArgs e)
            {
            // 1. Crear el Excel con el presupuesto actual
            using (var excel = new DOP.Presupuestos.Clases.Excel(Objeto))
                {
                // 2. Generar la hoja que desees (puedes cambiar el método)
                excel.PresupuestoEjecutivo();
                //excel.PresupuestoTipos(); // Genera la hoja de tipos de presupuesto
                // También podrías usar: excel.PresupuestoTipos(); o excel.PresupuestoComercial();

                // 3. Guardar el archivo temporalmente
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Presupuesto_{Guid.NewGuid():N}.xlsx");
                excel.book.SaveAs(tempPath);

                // 4. Intentar abrir el archivo con Excel
                try
                    {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                        FileName = tempPath,
                        UseShellExecute = true
                        });
                    }
                catch (Exception ex)
                    {
                    // Si no se pudo abrir, permitir al usuario elegir dónde guardar el archivo
                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                        {
                        Title = "Guardar archivo Excel",
                        Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
                        FileName = "Presupuesto.xlsx"
                        };

                    if (saveDialog.ShowDialog() == true)
                        {
                        try
                            {
                            File.Copy(tempPath, saveDialog.FileName, true);
                            MessageBox.Show($"Archivo guardado en:\n{saveDialog.FileName}", "Guardado exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        catch (Exception copyEx)
                            {
                            MessageBox.Show($"No se pudo guardar el archivo.\nError: {copyEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    else
                        {
                        MessageBox.Show($"No se pudo abrir Excel automáticamente.\nArchivo temporal generado en:\n{tempPath}\n\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }

        private void gMaestro_Click(object sender, RoutedEventArgs e)
            {
            //this.gPlanilla.Children.Add(Planilla);
            gLateral.Children.Clear();
            gLateral.Children.Add(Maestro);

            }

        private void gListado_Click(object sender, RoutedEventArgs e)
            {
            gLateral.Children.Clear();
            gLateral.Children.Add(Listado);
            //this.gListado.Children.Add(Listado);

            }

        private void cPrecios_Click(object sender, RoutedEventArgs e)
            {


            }

        private void gArbol_Click(object sender, RoutedEventArgs e)
            {
            if (Planilla.grillaArbol.View != null)
                {
                var boton = sender as RibbonButton;
                if (boton != null)
                    {
                    if (boton.Name == "gArbol")
                        {
                        // Quitar el filtro
                        Planilla.grillaArbol.View.Filter = null;
                        }
                    else if (boton.Name == "gFiltrado")
                        {
                        // Activar el filtro
                        Planilla.grillaArbol.View.Filter = Planilla.FiltrarPorTipo;
                        }
                    Planilla.grillaArbol.View.Refresh();
                    }
                }
            }

        private void RibbonCheckBox_Checked(object sender, RoutedEventArgs e)
            {
                gPrecios.Children.Add(Articulos);
            }
        }
    }
