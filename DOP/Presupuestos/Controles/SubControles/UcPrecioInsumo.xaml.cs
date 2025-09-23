using Biblioteca.DTO;
using DataObra.Interfaz.Ventanas;
using DOP.Datos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            gridBusquedaArticulos.PreviewMouseLeftButtonDown += GridBusquedaArticulos_PreviewMouseLeftButtonDown;
            gridArticulosInsumos.Drop += GridArticulosInsumos_Drop;
            gridArticulosInsumos.DragOver += GridArticulosInsumos_DragOver;
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

        private void GridBusquedaArticulos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
            var row = gridBusquedaArticulos.SelectedItem as ArticuloBusquedaDTO;
            if (row != null)
                {
                DragDrop.DoDragDrop(gridBusquedaArticulos, row, DragDropEffects.Copy);
                }
            }

        private void GridArticulosInsumos_DragOver(object sender, DragEventArgs e)
            {
            if (e.Data.GetDataPresent(typeof(ArticuloBusquedaDTO)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
            }

        private void GridArticulosInsumos_Drop(object sender, DragEventArgs e)
            {
            if (e.Data.GetDataPresent(typeof(ArticuloBusquedaDTO)))
                {
                var articuloBusqueda = e.Data.GetData(typeof(ArticuloBusquedaDTO)) as ArticuloBusquedaDTO;
                if (articuloBusqueda != null)
                    {
                    // Conversión de ArticuloBusquedaDTO a ArticuloInsumoDTO
                    var insumo = new ArticuloInsumoDTO
                        {
                        ArticuloID = articuloBusqueda.ID,
                        InsumoID = 0, // Asigna el valor adecuado si corresponde
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

                    // Evita duplicados si es necesario
                    if (!ArticulosInsumos.Any(x => x.ArticuloID == insumo.ArticuloID))
                        ArticulosInsumos.Add(insumo);
                    }
                }
            }
        }
    }
