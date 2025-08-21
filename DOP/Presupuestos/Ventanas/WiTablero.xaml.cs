using Biblioteca;
using Biblioteca.DTO;
using ClosedXML.Excel;
using DOP.Datos;
using DOP.Interfaz.Ventanas;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Lógica de interacción para WiTablero.xaml
    /// </summary>
    public partial class WiTablero : Window, INotifyPropertyChanged
        {
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private bool _isCustomMaximized = false;
        private ObservableCollection<PresupuestoDTO> _presupuestos = new();
        private ObservableCollection<PresupuestoDTO> _modelos = new();
        private ObservableCollection<PresupuestoDTO> _modelosPropios = new();

        public ObservableCollection<ListaArticuloItem> InfoCombo { get; set; } = new();

        public ObservableCollection<ArticuloDTO> ArticulosLista { get; set; } = new();

        public ObservableCollection<ArticuloBusquedaDTO> ArticulosBusqueda { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private List<ArticuloExceDTO> articulosImportados = new();
        public bool HayArticulosImportados => articulosImportados != null && articulosImportados.Count > 0;

        private void OnPropertyChanged(string propertyName)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        public WiTablero()
            {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "TabNavigationControl", "TabControlExt" }));
            InitializeComponent();
            this.DataContext = this; // ¡Esto es fundamental para el binding!
            //// Detectar la resolución de pantalla principal
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth <= 1920 && screenHeight <= 1080)
                {
                // Maximizar si la resolución es igual o menor a 1920x1080
                Maximize_Click(null, null);
                }
            else
                {
                // Centrar y establecer tamaño fijo si la resolución es mayor

                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowState = WindowState.Normal;
                Width = 1900;
                Height = 1030;
                }
            Loaded += WiTablero_Loaded;
            }

        private async void WiTablero_Loaded(object sender, RoutedEventArgs e)
            {
            // 1. Presupuestos
            var (success, message, lista) = await DatosWeb.ObtenerPresupuestosUsuarioAsync();
            if (success)
                {
                // Calcular ValorM2 para cada presupuesto
                foreach (var p in lista)
                    {
                    if (p.Superficie.HasValue && p.Superficie.Value > 0)
                        p.ValorM2 = p.PrEjecTotal / p.Superficie.Value;
                    else
                        p.ValorM2 = 0;
                    }

                _presupuestos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.UsuarioID == App.IdUsuario));
                _modelos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.EsModelo && p.UsuarioID == 4));
                _modelosPropios = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.EsModelo && p.UsuarioID == App.IdUsuario));

                GrillaPresupuestos.ItemsSource = _presupuestos;
                }
            else
                {
                MessageBox.Show($"No se pudieron cargar los presupuestos.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GrillaPresupuestos.ItemsSource = null;
                }

            // 2. Listas de artículos
            InfoCombo.Clear();
            int usuarioID = 4;
            var (okListas, msgListas, listas) = await DOP.Datos.DatosWeb.ObtenerListasArticulosPorUsuarioAsync(usuarioID);

            if (!okListas)
                {
                MessageBox.Show($"No se pudo obtener las listas: {msgListas}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            foreach (var listaItem in listas)
                {
                InfoCombo.Add(new ListaArticuloItem
                    {
                    ID = listaItem.ID,
                    Descrip = listaItem.Descrip
                    });
                }

            txtUsuario.Text = "Usuario: " + App.NombreUsuario;
            btnBackstage.Visibility = (App.tipoUsuario == 2)
                ? Visibility.Visible
                : Visibility.Collapsed;

            GraficoGraficoBarras();
            }

        #region Comportamiento ventana

        private void Maximize_Click(object sender, RoutedEventArgs e)
            {
            if (_isCustomMaximized)
                {
                // Restaurar el tamaño, la posición y la sombra anteriores
                WindowState = WindowState.Normal;
                Left = _previousLeft;
                Top = _previousTop;
                Width = _previousWidth;
                Height = _previousHeight;
                MainBorder.Margin = new Thickness(10);
                WindowShadow.Opacity = 0.5;
                _isCustomMaximized = false;
                }
            else
                {
                // Almacenar el tamaño y la posición actuales
                _previousLeft = Left;
                _previousTop = Top;
                _previousWidth = Width;
                _previousHeight = Height;

                // Maximizar la ventana y eliminar la sombra
                var screen = System.Windows.SystemParameters.WorkArea;
                Left = screen.Left;
                Top = screen.Top;
                Width = screen.Width;
                Height = screen.Height;
                MainBorder.Margin = new Thickness(0);
                WindowShadow.Opacity = 0;
                _isCustomMaximized = true;
                }
            }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            {
            WindowState = WindowState.Minimized;
            }

        private async void Close_Click(object sender, RoutedEventArgs e)
            {
            Close();
            // Obtener la MAC Address de la primera interfaz activa
            string macaddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                              nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault() ?? "UNKNOWN";

            // Registrar la salida en el log (si el usuario está logueado)
            if (App.IdUsuario > 0)
                {
                await DOP.Datos.DatosWeb.RegistrarSalidaUsuarioAsync(App.IdUsuario, macaddress);
                }
            Application.Current.Shutdown();
            }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
            if (e.ClickCount == 2)
                {
                Maximize_Click(sender, e);
                }
            else
                {
                DragMove();
                }
            }

        #endregion

        #region Grafico

        private void GraficoGraficoBarras()
            {
            // Borra series previas para evitar duplicados al recargar
            graficoBarras.Series.Clear();

            // Construye los datos a partir de la colección _modelos
            var datos = new ObservableCollection<DatoGrafico>(
                _modelos
                    .Where(m => !string.IsNullOrWhiteSpace(m.Descrip))
                    .Select(m => new DatoGrafico
                        {
                        Tipología = m.Descrip,
                        Importe = (double)m.ValorM2
                        })
            );

            // Ejes
            CategoryAxis primaryAxis = new CategoryAxis
                {
                Header = "Tipología",
                FontSize = 14
                };
            graficoBarras.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis
                {
                Header = "Valor del m2 (u$s)",
                FontSize = 14
                };
            graficoBarras.SecondaryAxis = secondaryAxis;

            // Leyenda
            ChartLegend legend = new ChartLegend();
            graficoBarras.Legend = legend;

            // Serie de columnas
            ColumnSeries series = new ColumnSeries
                {
                ItemsSource = datos,
                XBindingPath = "Tipología",
                YBindingPath = "Importe",
                ShowTooltip = true,
                Label = "Valor del m2"
                };

            graficoBarras.Series.Add(series);
            }

        #endregion

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
            {
            if (sender is MenuItem menuItem)
                {
                // Puedes usar Name o Header según cómo esté definido tu menú
                switch (menuItem.Header?.ToString())
                    {
                    case "Nuevo":
                        var ventana = new WiNuevoPres(_presupuestos, _modelos, _modelosPropios)
                            {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };
                        ventana.ShowDialog();
                        break;

                    case "Editar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                            {
                            var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                            if (ok)
                                {
                                var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado); // <-- aquí el cambio
                                var wiPresupuesto = new WiPresupuesto(copia, conceptos, relaciones, _presupuestos);
                                wiPresupuesto.Owner = this;
                                wiPresupuesto.ShowDialog();
                                }
                            else
                                {
                                MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    case "Borrar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                            {
                            var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                                {
                                var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                                if (success)
                                    {
                                    // Quitar de la colección y refrescar la grilla
                                    _presupuestos.Remove(eliminar);
                                    GrillaPresupuestos.ItemsSource = null;
                                    GrillaPresupuestos.ItemsSource = _presupuestos;
                                    MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                else
                                    {
                                    MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    default:
                        MessageBox.Show("Opción de menú no implementada.", "Menú", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                }
            }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var ventana = new WiNuevoPres(_presupuestos, _modelos, _modelosPropios)
                {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
            ventana.ShowDialog();
            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                {
                // Obtener conceptos y relaciones antes de abrir la ventana
                var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                if (ok)
                    {
                    // Aquí puedes pasar conceptos y relaciones a la ventana WiPresupuesto si lo necesitas
                    var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado);
                    var wiPresupuesto = new WiPresupuesto(copia, conceptos, relaciones, _presupuestos);
                    wiPresupuesto.Owner = this;
                    wiPresupuesto.ShowDialog();
                    // Si necesitas usar conceptos y relaciones después, puedes hacerlo aquí
                    }
                else
                    {
                    MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void btnBorrar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                {
                var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    {
                    var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                    if (success)
                        {
                        // Quitar de la colección y refrescar la grilla
                        _presupuestos.Remove(eliminar);
                        GrillaPresupuestos.ItemsSource = null;
                        GrillaPresupuestos.ItemsSource = _presupuestos;
                        MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    else
                        {
                        MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }

        private void btnBackstage_Click(object sender, RoutedEventArgs e)
            {

            }

        private void ExcelDropZone_DragEnter(object sender, DragEventArgs e)
            {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            }

        private void ExcelDropZone_DragLeave(object sender, DragEventArgs e)
            {
            // Opcional: feedback visual
            }

        private void ExcelDropZone_Drop(object sender, DragEventArgs e)
            {
            try
                {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                    var data = e.Data.GetData(DataFormats.FileDrop);
                    if (data is string[] files)
                        {
                        if (files.Length > 0 && System.IO.Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                            {
                            ImportarArticulosDesdeExcel(files[0]);
                            }
                        else
                            {
                            MessageBox.Show("Por favor, suelte un archivo Excel (.xlsx) válido.", "Archivo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show(
                    $"Ocurrió un error al importar el archivo:\n{ex.Message}",
                    "Error en importación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                }
            }

        private void ImportarArticulosDesdeExcel(string filePath)
            {
            var articulos = LeerArticulosDesdeExcel(filePath);
            if (articulos.Count > 0)
                {
                articulosImportados = articulos;
                OnPropertyChanged(nameof(HayArticulosImportados));
                MessageBox.Show($"Se importaron {articulosImportados.Count} artículos.", "Importación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show("No se importaron artículos. Verifique el archivo.", "Importación", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private List<ArticuloExceDTO> LeerArticulosDesdeExcel(string filePath)
            {
            var lista = new List<ArticuloExceDTO>();
            try
                {
                using (var workbook = new XLWorkbook(filePath))
                    {
                    var ws = workbook.Worksheets.FirstOrDefault();
                    if (ws == null)
                        {
                        MessageBox.Show("El archivo Excel no contiene hojas.", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                        return lista;
                        }

                    foreach (var row in ws.RowsUsed().Skip(1)) // Salta la cabecera
                        {
                        // Validar que las celdas existen y no son nulas
                        var cell1 = row.Cell(1);
                        var cell2 = row.Cell(2);
                        var cell3 = row.Cell(3);
                        var cell4 = row.Cell(4);

                        if (cell1 == null || cell2 == null || cell3 == null || cell4 == null)
                            continue;

                        var dto = new ArticuloExceDTO
                            {
                            Codigo = cell1.GetString() ?? "",
                            Descrip = cell2.GetString() ?? "",
                            Unidad = cell3.GetString() ?? "",
                            Precio = cell4.TryGetValue<decimal>(out var precio) ? precio : 0
                            };
                        lista.Add(dto);
                        }
                    }
                }
            catch (IOException)
                {
                MessageBox.Show("No se puede acceder al archivo porque está siendo usado por otro proceso. Por favor, cierre el archivo en Excel y vuelva a intentarlo.", "Archivo en uso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error inesperado al leer el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            return lista;
            }

        private async void BtnBuscarLista_Click(object sender, RoutedEventArgs e)
            {
            if (comboListasVer.SelectedValue is int id && id > 0)
                {
                var (success, message, articulos) = await DOP.Datos.DatosWeb.ObtenerArticulosPorListaIDAsync((short)id);
                if (success)
                    {
                    ArticulosLista.Clear();
                    foreach (var art in articulos)
                        ArticulosLista.Add(art);
                    GrillaArticulosLista.ItemsSource = ArticulosLista;
                    }
                else
                    {
                    MessageBox.Show($"No se pudieron obtener los artículos: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    GrillaArticulosLista.ItemsSource = null;
                    }
                }
            else
                {
                MessageBox.Show("Seleccione una lista válida.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void BorrarListaClick(object sender, RoutedEventArgs e)
            {
            if (comboListas.SelectedValue is int listaID && listaID > 0)
                {
                var result = MessageBox.Show("¿Está seguro que desea eliminar la lista seleccionada? Esta acción no se puede deshacer.", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                    {
                    var (success, message) = await DOP.Datos.DatosWeb.EliminarListaArticulosAsync(listaID);
                    if (success)
                        {
                        MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Refrescar las listas de artículos
                        int usuarioID = App.IdUsuario;
                        var (okListas, msgListas, listas) = await DOP.Datos.DatosWeb.ObtenerListasArticulosPorUsuarioAsync(4);
                        if (okListas)
                            {
                            InfoCombo.Clear();
                            foreach (var listaItem in listas)
                                {
                                InfoCombo.Add(new ListaArticuloItem
                                    {
                                    ID = listaItem.ID,
                                    Descrip = listaItem.Descrip
                                    });
                                }
                            }
                        else
                            {
                            MessageBox.Show($"No se pudo refrescar la lista de artículos: {msgListas}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    else
                        {
                        MessageBox.Show($"No se pudo eliminar la lista.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            else
                {
                MessageBox.Show("Seleccione una lista válida para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void ProcesarListaArticulos(object sender, RoutedEventArgs e)
            {
            if (comboListas.SelectedValue is int listaID && listaID > 0)
                {
                if (articulosImportados == null || articulosImportados.Count == 0)
                    {
                    MessageBox.Show("Primero debe importar artículos desde un archivo Excel.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                    }

                // Convertir ArticuloExceDTO a ArticuloDTO
                var articulosDTO = articulosImportados.Select(a => new ArticuloDTO
                    {
                    Codigo = a.Codigo,
                    Descrip = a.Descrip,
                    Unidad = string.IsNullOrEmpty(a.Unidad) ? "" : a.Unidad.Substring(0, Math.Min(2, a.Unidad.Length)),
                    Precio = a.Precio,
                    ListaID = (short)listaID,
                    // Completa los campos requeridos según tu lógica de negocio:
                    CuentaID = 0, // Ajusta si tienes el dato
                    UsuarioID = 4,
                    Fecha = DateTime.Now,
                    Moneda = "P", // O el valor que corresponda
                    UnidadFactor = 1
                    }).ToList();

                var (success, message) = await DatosWeb.ProcesarArticulosPorListaAsync(listaID, articulosDTO);

                if (success)
                    MessageBox.Show("Lista procesada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"No se pudo procesar la lista: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else
                {
                MessageBox.Show("Seleccione una lista válida.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private void ExcelDropZone_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
            var dialog = new Microsoft.Win32.OpenFileDialog
                {
                Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                Multiselect = false
                };

            if (dialog.ShowDialog() == true)
                {
                ImportarArticulosDesdeExcel(dialog.FileName);
                }
            }

        private async void NuevaLista_Click(object sender, RoutedEventArgs e)
            {
            var ventana = new WiNuevaListaArticulos
                {
                Owner = this
                };

            if (ventana.ShowDialog() == true)
                {
                var dto = ventana.NuevaLista;
                var (success, message, listaId) = await DatosWeb.CrearNuevaListaArticulosAsync(dto);

                if (success)
                    {
                    MessageBox.Show("Lista creada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Refresca las listas de artículos
                    int usuarioID = App.IdUsuario;
                    var (okListas, msgListas, listas) = await DatosWeb.ObtenerListasArticulosPorUsuarioAsync(4);
                    if (okListas)
                        {
                        InfoCombo.Clear();
                        foreach (var listaItem in listas)
                            {
                            InfoCombo.Add(new ListaArticuloItem
                                {
                                ID = listaItem.ID,
                                Descrip = listaItem.Descrip
                                });
                            }
                        }
                    }
                else
                    {
                    MessageBox.Show($"No se pudo crear la lista: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }



        private async void BtnBuscarPrecios_Click(object sender, RoutedEventArgs e)
            {
            string descripBusqueda = txtBusquedaArticulo.Text?.Trim() ?? "";
            string tipoSeleccionado = (comboTipoArticulo.SelectedItem as ComboBoxItem)?.Content as string;

            if (string.IsNullOrWhiteSpace(descripBusqueda))
                {
                MessageBox.Show("Ingrese una descripción para buscar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }
            if (string.IsNullOrWhiteSpace(tipoSeleccionado))
                {
                MessageBox.Show("Seleccione un tipo de artículo.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            // Mapear texto a letra
            string tipoID = tipoSeleccionado switch
                {
                    "Materiales" => "M",
                    "Mano de Obra" => "D",
                    "Equipos" => "E",
                    "Subcontratos" => "S",
                    "Otros" => "O",
                    _ => ""
                    };

            if (string.IsNullOrEmpty(tipoID))
                {
                MessageBox.Show("Tipo de artículo no válido.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            int usuarioID = 4;

            var (ok, msg, articulos) = await DatosWeb.BuscarArticulosAsync(usuarioID, tipoID, descripBusqueda);

            ArticulosBusqueda.Clear();
            if (ok && articulos != null)
                {
                foreach (var art in articulos)
                    ArticulosBusqueda.Add(art);
                }
            else
                {
                MessageBox.Show($"No se encontraron resultados.\n{msg}", "Búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }



        public class DatoGrafico
            {
            public string Tipología { get; set; }
            public double Importe
                {
                get; set;
                }
            }
        }
    }
