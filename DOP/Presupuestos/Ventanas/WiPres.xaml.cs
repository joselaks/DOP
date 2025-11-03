using Bibioteca.Clases;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DOP;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiPres.xaml
    /// </summary>
    public partial class WiPres : RibbonWindow
        {
        public Presupuesto Objeto;
        public UcPlanilla Planilla;
        public UcListado Listado;
        public UcDosaje Dosaje;
        public UcMaestro Maestro;
        public UcArticulos Articulos;
        public UcPlanillaListado PlanillaListado;
        public UcMaestroPrecios MaestroPrecios;
        private ContenedorPresupuesto _contenedor;
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;
        private GridLength? _detalleRow2Height;
        private GridLength? _listadoCol2Width;
        private GridLength? _maestroCol2Width;
        private bool _cierreSolicitadoPorUsuario = false;

        public WiPres(PresupuestoDTO? _encabezado, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, ObservableCollection<PresupuestoDTO> presupuestosRef)
            {
            InitializeComponent();
            Objeto = new Presupuesto(_encabezado, conceptos, relaciones);
            Objeto.encabezado.UsuarioID = App.IdUsuario;
            Dosaje = new UcDosaje(Objeto);
            Planilla = new UcPlanilla(Objeto, Dosaje);
            Listado = new UcListado(Objeto, Planilla);
            Maestro = new UcMaestro();
            Articulos = new UcArticulos();
            PlanillaListado = new UcPlanillaListado();
            MaestroPrecios = new UcMaestroPrecios();
            _contenedor = new ContenedorPresupuesto();
            this.gridPres.Children.Add(_contenedor);
            this.Loaded += WiPres_Loaded;
            this.Closing += WiPres_Closing;
            _presupuestosRef = presupuestosRef;

            this.DataContext = Objeto.encabezado;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth <= 1920 && screenHeight <= 1080)
                {
                // Maximizar si la resolución es igual o menor a 1920x1080

                WindowState = WindowState.Maximized;


                }
            else
                {
                // Centrar y establecer tamaño fijo si la resolución es mayor

                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowState = WindowState.Normal;
                Width = 1900;
                Height = 1030;
                }

            }

        private void InitializeCurrencyCombos()
            {
            // Asigna la colección global a los combos y muestra solo el nombre de la moneda
            moneda.ItemsSource = App.Monedas;
            moneda.DisplayMemberPath = nameof(Currency.Moneda);
            moneda.SelectedValuePath = nameof(Currency.Codigo);
            moneda.SelectionChanged += Moneda_SelectionChanged;

            moneda1.ItemsSource = App.Monedas;
            moneda1.DisplayMemberPath = nameof(Currency.Moneda);
            moneda1.SelectedValuePath = nameof(Currency.Codigo);
            moneda1.SelectionChanged += Moneda1_SelectionChanged;

            //moneda2.ItemsSource = App.Monedas;
            //moneda2.DisplayMemberPath = nameof(Currency.Moneda);
            //moneda2.SelectedValuePath = nameof(Currency.Codigo);
            //moneda2.SelectionChanged += Moneda2_SelectionChanged;
            }

        private void Moneda_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            var codigo = moneda.SelectedValue as string ?? (moneda.SelectedItem as Currency)?.Codigo;
            if (string.IsNullOrEmpty(codigo)) return;

            if (DataContext is PresupuestoDTO dto)
                dto.EjecMoneda = codigo[0];
            }
        private void Moneda1_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            var codigo = moneda1.SelectedValue as string ?? (moneda1.SelectedItem as Currency)?.Codigo;
            var nombreMoneda = (moneda1.SelectedItem as Currency)?.Moneda ?? string.Empty;
            if (string.IsNullOrEmpty(codigo)) return;

            if (DataContext is PresupuestoDTO dto)
                dto.EjecMoneda1 = codigo[0];

            // 10 decimales para peso argentino (código 'P' o nombre que contenga "peso"), 2 para el resto
            int decimales = (codigo.Equals("P", StringComparison.OrdinalIgnoreCase) ||
                             nombreMoneda.IndexOf("peso", StringComparison.OrdinalIgnoreCase) >= 0)
                             ? 10 : 2;

            if (nTipoCambio1 != null)
                {
                // Ajuste visual estándar
                nTipoCambio1.NumberDecimalDigits = decimales;

                // Intento de compatibilidad: si la versión de Syncfusion expone DecimalDigits/DecimalPlaces
                try
                    {
                    var t = nTipoCambio1.GetType();
                    var prop = t.GetProperty("DecimalDigits");
                    if (prop != null && prop.CanWrite)
                        prop.SetValue(nTipoCambio1, Convert.ChangeType(decimales, prop.PropertyType));
                    else
                        {
                        prop = t.GetProperty("DecimalPlaces");
                        if (prop != null && prop.CanWrite)
                            prop.SetValue(nTipoCambio1, Convert.ChangeType(decimales, prop.PropertyType));
                        }
                    }
                catch
                    {
                    // No fallar si la propiedad no existe en la versión de la librería
                    }
                }

            // Opcional: redondear el valor ligado en el DTO para coherencia visual
            if (DataContext is PresupuestoDTO dto2)
                {
                var pi = dto2.GetType().GetProperty("TipoCambioD");
                if (pi != null)
                    {
                    var val = pi.GetValue(dto2);
                    if (val is decimal decVal) pi.SetValue(dto2, Math.Round(decVal, decimales));
                    else if (val is double dblVal) pi.SetValue(dto2, Math.Round(dblVal, decimales));
                    }
                }
            }
        //private void Moneda2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //    {
        //    var codigo = moneda2.SelectedValue as string ?? (moneda2.SelectedItem as Currency)?.Codigo;
        //    if (string.IsNullOrEmpty(codigo)) return;

        //    if (DataContext is PresupuestoDTO dto)
        //        dto.EjecMoneda2 = codigo[0];
        //    }

        // Sincroniza la selección de los combos con los valores actuales en Objeto.encabezado
        private void SyncSelectionsFromModel()
            {
            var encabezado = Objeto?.encabezado;
            if (encabezado == null) return;

            // Asegurarse de que los ItemsSource ya están asignados
            // Usar string del char, o null si char es '\0'
            moneda.SelectedValue = encabezado.EjecMoneda != '\0' ? encabezado.EjecMoneda.ToString() : null;
            moneda1.SelectedValue = encabezado.EjecMoneda1 != '\0' ? encabezado.EjecMoneda1.ToString() : null;
            //moneda2.SelectedValue = encabezado.EjecMoneda2 != '\0' ? encabezado.EjecMoneda2.ToString() : null;
            }


        private void SolicitarCierre()
            {
            _cierreSolicitadoPorUsuario = true;
            this.Close();
            }


        private void WiPres_Closing(object? sender, CancelEventArgs e)
            {
            // Solo preguntar si el cierre NO fue solicitado explícitamente por el usuario desde los botones del Ribbon
            if (!_cierreSolicitadoPorUsuario)
                {
                var result = MessageBox.Show("¿Está seguro que desea cerrar el presupuesto?", "Confirmar cierre", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    {
                    e.Cancel = true; // Cancela el cierre
                    }
                }
            // Si _cierreSolicitadoPorUsuario es true, no pregunta y deja cerrar.
            }


        private void WiPres_Loaded(object sender, RoutedEventArgs e)
            {
            _contenedor.gridPlanilla.Children.Add(Planilla);
            _contenedor.gridListado.Children.Add(Listado);
            _contenedor.gridDetalle.Children.Add(Dosaje);
            gridMaestro.Children.Add(Maestro);
            InitializeCurrencyCombos();
            SyncSelectionsFromModel();
            }


        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {
            if (sender is RibbonButton boton)
                {
                switch (boton.Name)
                    {
                    case "BrnGuardar":
                        if (await GuardarPresupuestoAsync())
                            MessageBox.Show("Presupuesto guardado correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "BtnGuardarSalir":
                        var resultGuardarSalir = MessageBox.Show(
                            "¿Desea guardar los cambios y salir?",
                            "Confirmar",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
                        if (resultGuardarSalir == MessageBoxResult.Yes)
                            {
                            if (await GuardarPresupuestoAsync())
                                SolicitarCierre();
                            }
                        break;
                    case "BtnSalir":
                        var resultSalir = MessageBox.Show(
                            "¿Está seguro que desea salir sin guardar los cambios?",
                            "Confirmar salida",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
                        if (resultSalir == MessageBoxResult.Yes)
                            SolicitarCierre();
                        break;
                    }
                }
            }


        private async Task<bool> GuardarPresupuestoAsync()
            {
            // Actualiza totales y fechas
            Objeto.encabezado.PrEjecTotal = Objeto.Arbol.Sum(i => i.Importe1);
            Objeto.encabezado.FechaM = DateTime.Now;
            DateTime? selectedDate = pMesBase.Value;
            if (selectedDate.HasValue)
                {
                int mes = selectedDate.Value.Month;
                int año = selectedDate.Value.Year;
                int dia = 1;
                DateTime nuevaFecha = new DateTime(año, mes, dia);
                Objeto.encabezado.MesBase = nuevaFecha;
                }

            ProcesaPresupuestoRequest oGrabar = Objeto.EmpaquetarPresupuesto();

            // Llamada al servicio web para procesar el presupuesto
            var resultado = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(oGrabar);

            if (resultado.Success)
                {
                // Actualiza listas para próxima ejecución
                Objeto.listaConceptosLeer = Objeto.listaConceptosGrabar.Select(x => x).ToList();
                Objeto.listaRelacionesLeer = Objeto.listaRelacionesGrabar.Select(x => x).ToList();

                if (Objeto.encabezado.ID == null || Objeto.encabezado.ID == 0)
                    {
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
                    var existente = _presupuestosRef.FirstOrDefault(p => p.ID == Objeto.encabezado.ID);
                    if (existente != null)
                        {
                        existente.Descrip = Objeto.encabezado.Descrip;
                        existente.FechaC = Objeto.encabezado.FechaC;
                        existente.FechaM = Objeto.encabezado.FechaM;
                        existente.MesBase = Objeto.encabezado.MesBase;
                        existente.PrEjecTotal = Objeto.encabezado.PrEjecTotal;
                        existente.Superficie = Objeto.encabezado.Superficie;
                        existente.EsModelo = Objeto.encabezado.EsModelo;
                        existente.TipoCambioD = Objeto.encabezado.TipoCambioD;
                        //existente.TipoCambio1 = Objeto.encabezado.TipoCambio1;
                        //existente.TipoCambio2 = Objeto.encabezado.TipoCambio2;
                        existente.EjecMoneda = Objeto.encabezado.EjecMoneda;
                        existente.EjecMoneda1 = Objeto.encabezado.EjecMoneda1;
                        //existente.EjecMoneda2 = Objeto.encabezado.EjecMoneda2;

                        if (existente.Superficie.HasValue && existente.Superficie.Value > 0)
                            existente.ValorM2 = Math.Round(existente.PrEjecTotal / existente.Superficie.Value, 2);
                        else
                            existente.ValorM2 = 0;
                        }
                    else
                        {
                        _presupuestosRef.Add(Objeto.encabezado);
                        }
                    }
                return true;
                }
            else
                {
                MessageBox.Show($"Error al guardar el presupuesto: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
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

        private void btnDatos_Click(object sender, RoutedEventArgs e)
            {
            }

        private void Recalculo_Click(object sender, RoutedEventArgs e)
            {

            }


        private void ventanas_Checked(object sender, RoutedEventArgs e)
            {
            // Calcula la altura total del contenedor (asegúrate de que esté renderizado)
            double totalHeight = _contenedor.basePres.ActualHeight > 0
                ? _contenedor.basePres.ActualHeight
                : _contenedor.basePres.RenderSize.Height;

            // Si no está disponible, puedes forzar un update del layout
            if (totalHeight == 0)
                {
                _contenedor.basePres.UpdateLayout();
                totalHeight = _contenedor.basePres.ActualHeight > 0
                    ? _contenedor.basePres.ActualHeight
                    : _contenedor.basePres.RenderSize.Height;
                }

            // Calcula un tercio de la altura
            double alturaTercio = totalHeight / 3.0;


            // vDetalle: filas 2 y 3 de basePres
            if (sender is Syncfusion.Windows.Tools.Controls.RibbonCheckBox menuItemDetalle && menuItemDetalle.Name == "vDetalle")
                {
                if (_contenedor.basePres.RowDefinitions.Count >= 3)
                    {
                    if (menuItemDetalle.IsChecked == true)
                        {
                        _contenedor.basePres.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                        if (_detalleRow2Height != null)
                            {
                            _contenedor.basePres.RowDefinitions[2].Height = _detalleRow2Height.Value;
                            }
                        else
                            {
                            _contenedor.basePres.RowDefinitions[2].Height = new GridLength(alturaTercio, GridUnitType.Pixel);
                            }
                        }
                    else
                        {
                        _detalleRow2Height = _contenedor.basePres.RowDefinitions[2].Height;
                        _contenedor.basePres.RowDefinitions[1].Height = new GridLength(0);
                        _contenedor.basePres.RowDefinitions[2].Height = new GridLength(0);
                        }
                    }
                }

            // vListado: columnas 2 y 3 de basePres
            if (sender is Syncfusion.Windows.Tools.Controls.RibbonCheckBox menuItemListado && menuItemListado.Name == "vListado")
                {
                if (_contenedor.basePres.ColumnDefinitions.Count >= 3)
                    {
                    if (menuItemListado.IsChecked == true)
                        {
                        _contenedor.basePres.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        _contenedor.basePres.ColumnDefinitions[2].Width = _listadoCol2Width ?? new GridLength(700);
                        if (vMaestro.IsChecked == true)
                            {
                            vMaestro.IsChecked = false;
                            }
                        }

                    else
                        {
                        _listadoCol2Width = _contenedor.basePres.ColumnDefinitions[2].Width;
                        _contenedor.basePres.ColumnDefinitions[1].Width = new GridLength(0);
                        _contenedor.basePres.ColumnDefinitions[2].Width = new GridLength(0);
                        }
                    }
                }

            // vMaestro: columnas 1 y 2 de gridBase
            if (sender is Syncfusion.Windows.Tools.Controls.RibbonCheckBox menuItemMaestro && menuItemMaestro.Name == "vMaestro")
                {
                if (gridBase.ColumnDefinitions.Count >= 3)
                    {
                    if (menuItemMaestro.IsChecked == true)
                        {
                        gridBase.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        gridBase.ColumnDefinitions[2].Width = _maestroCol2Width ?? new GridLength(600);
                        if (vListado.IsChecked == true)
                            {
                            vListado.IsChecked = false;
                            }
                        }
                    else
                        {
                        _maestroCol2Width = gridBase.ColumnDefinitions[2].Width;
                        gridBase.ColumnDefinitions[1].Width = new GridLength(0);
                        gridBase.ColumnDefinitions[2].Width = new GridLength(0);
                        }
                    }
                }
            }

        private void Incidencias_Click(object sender, RoutedEventArgs e)
            {
            Objeto.CalcularIncidencia();
            }

        private void Renumerar_Click(object sender, RoutedEventArgs e)
            {
            Objeto.NumeraItems(Objeto.Arbol, "");

            }

        private void Hacer_Click(object sender, RoutedEventArgs e)
            {
            Objeto.Rehacer();
            }

        private void Deshacer_Click(object sender, RoutedEventArgs e)
            {
            Objeto.Deshacer();
            }


        private static readonly Regex _regex = new Regex(@"^\d*([.,]\d{0,2})?$");

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            {
            TextBox textBox = sender as TextBox;
            string fullText = GetFullTextAfterInput(textBox, e.Text);
            e.Handled = !_regex.IsMatch(fullText);
            }

        private void DecimalTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
            {
            if (e.DataObject.GetDataPresent(typeof(string)))
                {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                TextBox textBox = sender as TextBox;
                string fullText = GetFullTextAfterInput(textBox, pastedText);
                if (!_regex.IsMatch(fullText))
                    {
                    e.CancelCommand();
                    }
                }
            else
                {
                e.CancelCommand();
                }
            }

        public void SetMultimonedaEnabled(bool enabled)
            {
            // Asegura ejecución en el hilo de UI
            if (!Dispatcher.CheckAccess())
                {
                Dispatcher.Invoke(() => SetMultimonedaEnabled(enabled));
                return;
                }

            if (txtMonedaBase != null)
                {
                txtMonedaBase.Opacity = enabled ? 1.0 : 0.6;
                }

            if (moneda != null)
                {
                moneda.IsEnabled = enabled;
                moneda.IsHitTestVisible = enabled;
                moneda.Opacity = enabled ? 1.0 : 0.6;
                }

            if (moneda1 != null)
                {
                moneda1.IsEnabled = enabled;
                moneda1.IsHitTestVisible = enabled;
                moneda1.Opacity = enabled ? 1.0 : 0.6;
                }

            if (nTipoCambio1 != null)
                {
                nTipoCambio1.IsEnabled = enabled;
                nTipoCambio1.IsHitTestVisible = enabled;
                nTipoCambio1.Opacity = enabled ? 1.0 : 0.6;
                }
            }



        private string GetFullTextAfterInput(TextBox textBox, string input)
        {
        string currentText = textBox.Text;
        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;
        return currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, input);
        }

        private void chkBimonetario_Checked(object sender, RoutedEventArgs e)
            {
                SetMultimonedaEnabled(true);
            }


        private void chkBimonetario_Unchecked(object sender, RoutedEventArgs e)
            {
            SetMultimonedaEnabled(false);
            }

    }
}
