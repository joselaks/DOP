using Biblioteca.DTO;
using ClosedXML.Excel;
using DataObra.Interfaz.Ventanas;
using DOP;
using DOP.Datos;
using DOP.Presupuestos.Ventanas;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xPrecios.xaml
    /// </summary>
    public partial class xPrecios : UserControl, INotifyPropertyChanged
        {
        private WiEscritorio escritorio;
        private List<ArticuloExceDTO> articulosImportados = new();
        public bool HayArticulosImportados => articulosImportados != null && articulosImportados.Count > 0;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        public xPrecios(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
            this.DataContext = escritorio;
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPrecios", "Normal");
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
                    escritorio.ArticulosLista.Clear();
                    foreach (var art in articulos)
                        escritorio.ArticulosLista.Add(art);
                    GrillaArticulosLista.ItemsSource = escritorio.ArticulosLista;
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
                            escritorio.InfoCombo.Clear();
                            foreach (var listaItem in listas)
                                {
                                escritorio.InfoCombo.Add(new ListaArticuloItem
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
                Owner = escritorio
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
                        escritorio.InfoCombo.Clear();
                        foreach (var listaItem in listas)
                            {
                            escritorio.InfoCombo.Add(new ListaArticuloItem
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
            if (descripBusqueda.Length < 3)
                {
                MessageBox.Show("La búsqueda debe tener al menos tres caracteres.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            escritorio.ArticulosBusqueda.Clear();
            if (ok && articulos != null)
                {
                foreach (var art in articulos)
                    escritorio.ArticulosBusqueda.Add(art);
                }
            else
                {
                MessageBox.Show($"No se encontraron resultados.\n{msg}", "Búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }


        }
}
