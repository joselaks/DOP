using Biblioteca.DTO;
using DOP.Datos;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Presupuestos.Controles.SubControles
    {
    /// <summary>
    /// Lógica de interacción para UcPrecioInsumo.xaml
    /// </summary>
    public partial class UcPrecioInsumo : UserControl
        {
        public ObservableCollection<ArticuloBusquedaDTO> ArticulosBusqueda { get; set; } = new();
        public ObservableCollection<ArticuloInsumoDTO> ArticulosInsumos { get; set; } = new();

        public UcPrecioInsumo()
            {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += UcPrecioInsumo_Loaded;
            }

        private void UcPrecioInsumo_Loaded(object sender, RoutedEventArgs e)
            {
            // Suscribir eventos aquí, cuando los controles ya están inicializados
            gridBusquedaArticulos.RowDragDropController.DragStart += GridBusquedaArticulos_DragStart;
            gridArticulosInsumos.RowDragDropController.Drop += GridArticulosInsumos_Drop;
            gridArticulosInsumos.CurrentCellValueChanged += GridArticulosInsumos_CurrentCellValueChanged;
            }

        private void GridBusquedaArticulos_DragStart(object? sender, GridRowDragStartEventArgs e)
            {
            // Aquí puedes personalizar el inicio del drag si lo necesitas.
            // Por ejemplo, puedes cancelar el drag para ciertas filas:
            // if (e.RowData is ArticuloBusquedaDTO art && !art.PermitirDrag)
            //     e.Cancel = true;
            }

        private void GridArticulosInsumos_Drop(object sender, GridRowDropEventArgs e)
            {
            if (e.IsFromOutSideSource)
                {
                ObservableCollection<object> DraggingRecords = new ObservableCollection<object>();
                DraggingRecords = e.Data.GetData("Records") as ObservableCollection<object>;
                if (DraggingRecords != null)
                    {
                    foreach (var item in DraggingRecords)
                        {
                        if (item is ArticuloBusquedaDTO articuloBusqueda)
                            AgregarInsumoDesdeBusqueda(articuloBusqueda);
                        }
                    }

                }
            }
        private void GridArticulosInsumos_CurrentCellValueChanged(object? sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
            {
            if (e.Column.MappingName == "Seleccionado")
                {
                var grid = sender as SfDataGrid;
                if (grid?.View == null) return;

                // Obtiene el registro afectado
                var changedItem = grid.View.Records[e.RowColumnIndex.RowIndex - 1].Data as ArticuloInsumoDTO;
                if (changedItem != null && changedItem.Seleccionado)
                    {
                    foreach (var item in ArticulosInsumos)
                        {
                        if (!ReferenceEquals(item, changedItem) && item.Seleccionado)
                            item.Seleccionado = false;
                        }
                    }
                }
            }


        private void AgregarInsumoDesdeBusqueda(ArticuloBusquedaDTO articuloBusqueda)
            {
            var insumo = new ArticuloInsumoDTO
                {
                ArticuloID = articuloBusqueda.ID,
                InsumoID = 0,
                Descrip = articuloBusqueda.Descrip,
                TipoID = articuloBusqueda.TipoID,
                Unidad = articuloBusqueda.Unidad,
                FactorPrecio = articuloBusqueda.Precio,
                FactorUnidad = articuloBusqueda.UnidadFactor,
                Seleccionado = false,
                Fecha = articuloBusqueda.Fecha,
                Moneda = articuloBusqueda.Moneda,
                Nota = articuloBusqueda.Nota,
                URL = articuloBusqueda.URL,
                ListaDescrip = articuloBusqueda.ListaDescrip
                };

            if (!ArticulosInsumos.Any(x => x.ArticuloID == insumo.ArticuloID))
                ArticulosInsumos.Add(insumo);
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
        }
    }