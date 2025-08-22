using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Ventanas;
using DOP.Datos;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.TreeGrid;
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
using System.Windows.Controls.Primitives;
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
        public UcPlanillaListado PlanillaListado;
        public UcMaestroPrecios MaestroPrecios;
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;
        private GridLength? _panelDetalleUserHeight = null;
        private GridLength? _panelListadoUserWidth = null;
        private GridLength? _panelMaestroUserWidth = null;
        private GridLength? _panelPreciosUserWidth = null;
        private GridLength? _sepMaestroUserWidth = null;
        public GridLength? _panMaestroUserWidth = null;



        public WiPresupuesto(PresupuestoDTO? _encabezado, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, ObservableCollection<PresupuestoDTO> presupuestosRef)
            {
            InitializeComponent();
            Objeto = new Presupuesto(_encabezado, conceptos, relaciones);

            Objeto.encabezado.UsuarioID = App.IdUsuario;

            Dosaje = new UcDosaje(Objeto);
            Planilla = new UcPlanilla(Objeto, Dosaje);
            Listado = new UcListado(Objeto);
            Maestro = new UcMaestro();
            Articulos = new UcArticulos();
            PlanillaListado = new UcPlanillaListado();
            MaestroPrecios = new UcMaestroPrecios();
            MaestroPrecios.gridMaestro.Children.Add(Maestro);
            MaestroPrecios.gridPrecios.Children.Add(Articulos);

            this.gPlanilla.Children.Add(Planilla);
            this.panelDetalle.Children.Add(Dosaje);

            this.panelMaestro.Children.Add(MaestroPrecios);


            this.Loaded += WiPresupuesto_Loaded;

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
            MaestroPrecios.sepPrecio.Width = new GridLength(0);
            MaestroPrecios.panPrecio.Width = new GridLength(0);



            }


        private void WiPresupuesto_Loaded(object sender, RoutedEventArgs e)
            {
            // Desuscribir el evento antes de cambiar el estado
            colCodigo.IsCheckedChanged -= columnas_IsCheckedChanged;
            colTipo.IsCheckedChanged -= columnas_IsCheckedChanged;
            // ... repite para los que quieras dejar checked ...

            // Establecer los valores iniciales
            colMat.IsChecked = true;
            colMDO.IsChecked = true;
            // ... repite para los que quieras dejar checked ...

            // Volver a suscribir el evento
            colCodigo.IsCheckedChanged += columnas_IsCheckedChanged;
            colTipo.IsCheckedChanged += columnas_IsCheckedChanged;
            // ... repite para los que corresponda ...

   //         dropColumnasMaestro.Visibility = (App.tipoUsuario == 2)
   //? Visibility.Visible
   //: Visibility.Collapsed;
   //         chkPrecios.Visibility = (App.tipoUsuario == 2)
   //? Visibility.Visible
   //: Visibility.Collapsed;


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

            Objeto.encabezado.PrEjecTotal = Objeto.Arbol.Sum(i => i.Importe1);
            Objeto.encabezado.FechaM = DateTime.Now;

            ProcesaPresupuestoRequest oGrabar = Objeto.EmpaquetarPresupuesto();

            // Llamada al servicio web para procesar el presupuesto
            var resultado = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(oGrabar);

            if (resultado.Success)
                {
                // --- ACTUALIZACIÓN DE LISTAS PARA PRÓXIMA EJECUCIÓN ---
                Objeto.listaConceptosLeer = Objeto.listaConceptosGrabar.Select(x => x).ToList();
                Objeto.listaRelacionesLeer = Objeto.listaRelacionesGrabar.Select(x => x).ToList();

                MessageBox.Show($"Presupuesto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);


                if (Objeto.encabezado.ID == null || Objeto.encabezado.ID==0)
                    {
                    // Nuevo presupuesto: asignar fecha de creación y ID, luego agregar a la colección
                    Objeto.encabezado.FechaC = DateTime.Today;
                    Objeto.encabezado.ID = resultado.PresupuestoID;
                    var superficie = Objeto.encabezado.Superficie ?? 0;
                    if (Objeto.encabezado.Superficie.HasValue && Objeto.encabezado.Superficie.Value > 0)
                        Objeto.encabezado.ValorM2 = Math.Round(Objeto.encabezado.PrEjecTotal / Objeto.encabezado.Superficie.Value, 2);
                    else
                        Objeto.encabezado.ValorM2 = 0;

                    _presupuestosRef.Add(PresupuestoDTO.CopiarPresupuestoDTO(Objeto.encabezado));
                    }
                else
                    {
                    // Edición: buscar y reemplazar en la colección
                    var existente = _presupuestosRef.FirstOrDefault(p => p.ID == Objeto.encabezado.ID);
                    if (existente != null)
                        {
                        // Copia los valores de Objeto.encabezado (la copia) al objeto original
                        existente.Descrip = Objeto.encabezado.Descrip;
                        existente.FechaC = Objeto.encabezado.FechaC;
                        existente.FechaM = Objeto.encabezado.FechaM;
                        existente.MesBase = Objeto.encabezado.MesBase;
                        existente.PrEjecTotal = Objeto.encabezado.PrEjecTotal;
                        existente.Superficie = Objeto.encabezado.Superficie;
                        existente.EsModelo= Objeto.encabezado.EsModelo;

                        // Recalcular ValorM2 de forma segura
                        if (existente.Superficie.HasValue && existente.Superficie.Value > 0)
                            existente.ValorM2 = Math.Round(existente.PrEjecTotal / existente.Superficie.Value, 2);
                        else
                            existente.ValorM2 = 0;



                        // ... y el resto de propiedades
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
                Objeto.RecalculoCompleto();
                }

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


                                // Obtener la vista del árbol
                                var view = Planilla.grillaArbol.View;
                                if (view != null && sele != null)
                                    {
                                    var node = FindNodeByData(view.Nodes, sele);
                                    if (node != null && !node.IsExpanded)
                                        {
                                        Planilla.grillaArbol.ExpandNode(node);
                                        }
                                    }
                                }
                            }
                        break;
                    default:
                        // Otro caso
                        break;
                    }
                }
            }

        private TreeNode? FindNodeByData(IEnumerable<TreeNode> nodes, object data)
            {
            foreach (var node in nodes)
                {
                if (node.Item == data) // Cambia aquí 'Item' si es necesario
                    return node;
                var found = FindNodeByData(node.ChildNodes, data);
                if (found != null)
                    return found;
                }
            return null;
            }

        private void Recalculo_Click(object sender, RoutedEventArgs e)
            {
            Objeto.RecalculoCompleto();

            }

        private void Recnumerar_Click(object sender, RoutedEventArgs e)
            {
            Objeto.NumeraItems(Objeto.Arbol, "");
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
            ////this.gPlanilla.Children.Add(Planilla);
            //gLateral.Children.Clear();
            //gLateral.Children.Add(Maestro);

            }

        private void gListado_Click(object sender, RoutedEventArgs e)
            {
            //gLateral.Children.Clear();
            //gLateral.Children.Add(Listado);
            ////this.gListado.Children.Add(Listado);

            }

        private void cPrecios_Click(object sender, RoutedEventArgs e)
            {


            }


        private void expandir_Click(object sender, RoutedEventArgs e)
            {
            if (Planilla.grillaArbol.View != null)
                {
                var boton = sender as RibbonButton;
                if (boton != null)
                    {
                    if (boton.Name == "Expandir")
                        {
                        Planilla.ExpandeRubro();
                        }
                    else if (boton.Name == "Contraer")
                        {
                        Planilla.grillaArbol.CollapseAllNodes();
                        }
                    }
                }
            }

        private void VentanaListado_Checked(object sender, RoutedEventArgs e)
            {
            desmarca();
            // Si ya está UcPlanillaListado, no hacer nada
            if (gPlanilla.Children.Count == 1 && gPlanilla.Children[0] == PlanillaListado)
                return;

            // Quitar Planilla y Listado de cualquier contenedor anterior
            if (Planilla.Parent is Panel parentPanel1)
                parentPanel1.Children.Remove(Planilla);
            if (Listado.Parent is Panel parentPanel2)
                parentPanel2.Children.Remove(Listado);

            // Limpiar los grids internos de PlanillaListado
            PlanillaListado.gridPlanilla.Children.Clear();
            PlanillaListado.gridListado.Children.Clear();

            // Agregar Planilla y Listado a los grids internos
            PlanillaListado.gridPlanilla.Children.Add(Planilla);
            PlanillaListado.gridListado.Children.Add(Listado);

            // Limpiar el contenedor principal y agregar PlanillaListado
            gPlanilla.Children.Clear();
            gPlanilla.Children.Add(PlanillaListado);

            }

        private void VentanaListado_Unchecked(object sender, RoutedEventArgs e)
            {
            desmarca();
            // Si ya está Planilla, no hacer nada
            if (gPlanilla.Children.Count == 1 && gPlanilla.Children[0] == Planilla)
                return;

            // Quitar Planilla de cualquier contenedor anterior
            if (Planilla.Parent is Panel parentPanel)
                parentPanel.Children.Remove(Planilla);

            gPlanilla.Children.Clear();
            gPlanilla.Children.Add(Planilla);
            }



        private void VentanaDetalle_Checked(object sender, RoutedEventArgs e)
            {
            desmarca();
            basePres.RowDefinitions[1].Height = GridLength.Auto;   // sepDetalle
            var row = basePres.RowDefinitions[2];
            if (row.Height.Value == 0)
                {
                row.Height = _panelDetalleUserHeight ?? new GridLength(200);
                }
            }

        private void VentanaDetalle_Unchecked(object sender, RoutedEventArgs e)
            {
            desmarca();
            basePres.RowDefinitions[1].Height = new GridLength(0); // sepDetalle
            var row = basePres.RowDefinitions[2];
            if (row.Height.Value != 0)
                _panelDetalleUserHeight = row.Height;
            row.Height = new GridLength(0);
            }

        private void Maestro_Checked(object sender, RoutedEventArgs e)
            {
            desmarca();

            sepMaestro.Width = GridLength.Auto;
            panMaestro.Width = _panMaestroUserWidth ?? new GridLength(500);
            }

        private void Maestro_Unchecked(object sender, RoutedEventArgs e)
            {
            desmarca();

            // Guardar el ancho actual antes de ocultar
            _panMaestroUserWidth = panMaestro.Width;

            sepMaestro.Width = new GridLength(0);
            panMaestro.Width = new GridLength(0);
            }

        private void PresupClick(object sender, RoutedEventArgs e)
            {
            // Guardar el origen del click
            string sele = "";
            var origen = sender as ToggleButton;
            // Si no es ToggleButton (por ejemplo, un Button normal), puedes adaptarlo según tu caso
            //if (rbSoloPlanilla.IsChecked == true) sele= "Planilla";
            if (rbPlanillaListado.IsChecked == true) sele = "Listado";
            if (rbPresupuestoCompleto.IsChecked == true) sele = "Maestro";


            chkDetalle.IsChecked = false;
            chkMaestro.IsChecked = false;
            chkPrecios.IsChecked = false;
            chkListado.IsChecked = false;

            // Si se selecciona PlanillaListado (ambos) o PresupuestoCompleto
            if (sele == "Listado" || sele == "Maestro")
                {

                // Si es PresupuestoCompleto, marcar Maestro
                if (sele == "Maestro")
                    {
                    chkMaestro.IsChecked = true;
                    }

                chkDetalle.IsChecked = true;


                chkDetalle.IsChecked = true;
                chkPrecios.IsChecked = true;
                chkListado.IsChecked = true;
                }
            // Volver a marcar el origen al final
            if (origen != null) origen.IsChecked = true;
            }


        private void Combo_Click(object sender, RoutedEventArgs e)
            {
            if (sender is RadioButton radio)
                {
                switch (radio.Name)
                    {
                    case "rbPlanilla":
                        chkDetalle.IsChecked = false;
                        chkMaestro.IsChecked = false;
                        chkPrecios.IsChecked = false;
                        radio.IsChecked = true;
                        break;

                    case "rbPresupuesto":
                        chkDetalle.IsChecked = true;
                        chkMaestro.IsChecked = false;
                        chkPrecios.IsChecked = false;
                        radio.IsChecked = true;
                        break;

                    case "rbMaestro":
                        chkDetalle.IsChecked = true;
                        chkMaestro.IsChecked = true;
                        chkPrecios.IsChecked = false;
                        radio.IsChecked = true;
                        break;

                    default:
                        // Si hay más RadioButton, añade aquí su lógica
                        break;
                    }
                }
            }

        private void columnas_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            // Determinar qué DropDownMenuItem disparó el evento
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItem)
                {
                // Mapeo entre x:Name del menú y MappingName de la columna
                var mapping = new Dictionary<string, string>
        {
            { "colCodigo", "ID" },
            { "colTipo", "Tipo" },
            { "colMat", "Materiales1" },
            { "colMDO", "ManodeObra1" },
            { "colEqi", "Equipos1" },
            { "colSub", "Subcontratos1" },
            { "colOtr", "Otros1" }
        };

                if (mapping.TryGetValue(menuItem.Name, out string mappingName))
                    {
                    var column = Planilla.grillaArbol.Columns.FirstOrDefault(c => c.MappingName == mappingName);
                    if (column != null)
                        {
                        column.IsHidden = !menuItem.IsChecked;
                        }
                    }
                }
            }


        private void columnasDetalle_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            // Determinar qué DropDownMenuItem disparó el evento
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItem)
                {
                // Mapeo entre x:Name del menú y MappingName de la columna
                var mapping = new Dictionary<string, string>
        {
            { "colCodigoDetalle", "ID" },
            { "colTipoDetalle", "Tipo" },
            { "colMatDetalle", "Materiales1" },
            { "colMDODetalle", "ManodeObra1" },
            { "colEqiDetalle", "Equipos1" },
            { "colSubDetalle", "Subcontratos1" },
            { "colOtrDetalle", "Otros1" }
        };

                if (mapping.TryGetValue(menuItem.Name, out string nombreCOL))
                    {
                    var column = Dosaje.grillaDetalle.Columns.FirstOrDefault(c => c.MappingName == nombreCOL);
                    if (column != null)
                        {
                        column.IsHidden = !menuItem.IsChecked;
                        }
                    }
                }
            }

        private void columnasMaestro_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            // Determinar qué DropDownMenuItem disparó el evento
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItem)
                {
                // Mapeo entre x:Name del menú y MappingName de la columna
                var mapping = new Dictionary<string, string>
        {
            { "colCodigoMaestro", "ID" },
            { "colTipoMaestro", "Tipo" },
            
        };

                if (mapping.TryGetValue(menuItem.Name, out string nombreCOL))
                    {
                    var column = Maestro.grillaMaestro.Columns.FirstOrDefault(c => c.MappingName == nombreCOL);
                    if (column != null)
                        {
                        column.IsHidden = !menuItem.IsChecked;
                        }
                    }
                }
            }






        private void chkArbol_Checked(object sender, RoutedEventArgs e)
            {
            if (Planilla.grillaArbol.View != null)
                {
                // Quitar el filtro
                Planilla.grillaArbol.View.Filter = null;
                Planilla.grillaArbol.View.Refresh();
                Planilla.ExpandeRubro();
                }
            }


        private void chkArbol_Unchecked(object sender, RoutedEventArgs e)
            {

            if (Planilla.grillaArbol.View != null)
                {
                // Activar el filtro
                Planilla.grillaArbol.View.Filter = Planilla.FiltrarPorTipo;
                Planilla.grillaArbol.View.Refresh();
                Planilla.ExpandeRubro();
                }

            }

        private void desmarca()
            {
            // Desmarcar todos los RadioButton del grupo ComboGroup
            var radioNames = new[] { "rbSoloPlanilla", "rbPlanillaListado", "rbPresupuestoCompleto" };
            foreach (var name in radioNames)
                {
                var radio = this.FindName(name) as RadioButton;
                if (radio != null)
                    radio.IsChecked = false;
                }


            }

        private void chkPrecios_Checked(object sender, RoutedEventArgs e)
            {
            MaestroPrecios.sepPrecio.Width = GridLength.Auto; // Para "Auto"
            MaestroPrecios.panPrecio.Width = new GridLength(1, GridUnitType.Star); // Para "*"
            }

        private void chkPrecios_Unchecked(object sender, RoutedEventArgs e)
            {
            MaestroPrecios.sepPrecio.Width = new GridLength(0);
            MaestroPrecios.panPrecio.Width = new GridLength(0);

            }


        private void comboTipoListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            var combo = sender as Syncfusion.Windows.Tools.Controls.RibbonComboBox;
            var selectedItem = combo?.SelectedItem as Syncfusion.Windows.Tools.Controls.RibbonComboBoxItem;
            string seleccion = selectedItem?.Content?.ToString();

            // Verifica que Listado no sea null antes de llamar al método
            if (Listado != null && !string.IsNullOrEmpty(seleccion))
                {

                //Listado.comboTipoListado_SelectionChanged(seleccion);
                }
            }

        private void btnDatos_Click(object sender, RoutedEventArgs e)
            {
            var dlg = new WiDatosGenerales(Objeto.encabezado)
                {
                Owner = this
                };
            if (dlg.ShowDialog() == true)
                {
                // Los cambios ya están reflejados en Objeto.encabezado por el binding
                // Si necesitas notificar cambios manualmente, hazlo aquí
                }
            }

        private void comboTipoMaestro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as Syncfusion.Windows.Tools.Controls.RibbonComboBox;
            var selectedItem = combo?.SelectedItem as Syncfusion.Windows.Tools.Controls.RibbonComboBoxItem;
            string seleccion = selectedItem?.Content?.ToString();

            // Verifica que Listado no sea null antes de llamar al método
            if (Maestro != null && !string.IsNullOrEmpty(seleccion))
            {
                Maestro.SelectorTipo_SelectionChanged(seleccion);

                //Listado.comboTipoListado_SelectionChanged(seleccion);
            }

        }
    }
    }





















