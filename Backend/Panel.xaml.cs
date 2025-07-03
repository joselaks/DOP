using Backend.Datos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biblioteca.DTO;

namespace Backend
{
    public partial class Panel : Window
    {
        private List<InsumoDTO> Insumos = new();
        private List<Articulo> Articulos = new();
        private List<Tarea> Tareas = new();
        private List<Articulo> ArticulosProveedor = new();
        private List<Rubro> Rubros = new();

        private readonly Conectores meconecto = new();

        public Panel()
        {
            InitializeComponent();
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;
            cmbTipoInsumo.SelectionChanged += cmbTipoInsumo_SelectionChanged;
            _ = CargarInsumos();
            ListaArticulos.ItemsSource = Articulos;
            ListaTareas.ItemsSource = Tareas;
            ListaArticulosProveedor.ItemsSource = ArticulosProveedor;
        }

        private async Task CargarInsumos()
        {
            var (ok, msg, datos) = await meconecto.ObtenerInsumosPorUsuarioAsync(1);
            if (ok && datos != null)
            {
                foreach (var d in datos)
                    d.TipoDescripcion = ConvertirTipo(d.Tipo);

                Insumos = datos;
                FiltrarInsumos();
            }
            else
            {
                MessageBox.Show("Error al cargar insumos: " + msg);
            }
        }

        private string ConvertirTipo(string tipo)
        {
            return tipo switch
            {
                "M" => "Material",
                "E" => "Equipo",
                "C" => "SubContrato",
                "O" => "Mano de Obra",
                "T" => "Otros",
                _ => tipo
            };
        }

        private void BtnLimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {
            txtBuscarInsumo.Text = string.Empty;
            cmbTipoInsumo.SelectedIndex = 0;
        }

        private void FiltrarInsumos()
        {
            string filtro = txtBuscarInsumo?.Text?.ToLower() ?? string.Empty;
            string tipoFiltro = ((ComboBoxItem)cmbTipoInsumo.SelectedItem)?.Content?.ToString();

            var filtrados = Insumos.Where(i =>
                (string.IsNullOrEmpty(tipoFiltro) || tipoFiltro == "Todos" || ConvertirTipo(i.Tipo) == tipoFiltro) &&
                i.Descrip?.ToLower().Contains(filtro) == true
            ).ToList();

            if (ListaInsumos != null)
                {
                ListaInsumos.ItemsSource = null;
                ListaInsumos.ItemsSource = filtrados;
                }
        }

        private void TxtBuscarInsumo_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarInsumos();
        }

        private void cmbTipoInsumo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltrarInsumos();
        }

        private void TxtBuscarTarea_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarTarea.Text.ToLower();
            ListaTareas.ItemsSource = Tareas.Where(t => t.Descripcion.ToLower().Contains(filtro)).ToList();
        }

        private void TxtBuscarArticuloProveedor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarArticuloProveedor.Text.ToLower();
            ListaArticulosProveedor.ItemsSource = ArticulosProveedor.Where(a => a.Descripcion.ToLower().Contains(filtro)).ToList();
        }

        private void BtnActualizarArticulos_Click(object sender, RoutedEventArgs e)
        {
            if (ListaInsumos.SelectedItem is InsumoDTO insumo)
            {
                ListaArticulos.ItemsSource = ArticulosProveedor.Where(a => a.Descripcion.ToLower().Contains(insumo.Descrip.ToLower())).ToList();
            }
        }

        private void BtnActualizarInsumosDesdeTarea_Click(object sender, RoutedEventArgs e)
        {
            if (ListaTareas.SelectedItem is Tarea tarea)
            {
                ListaInsumos.ItemsSource = Insumos.Where(i => i.Descrip.ToLower().Contains(tarea.Descripcion.ToLower())).ToList();
            }
        }

        private void BtnAgregarArticuloDesdeProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (ListaArticulosProveedor.SelectedItem is Articulo art)
            {
                var lista = ListaArticulos.ItemsSource as List<Articulo> ?? new List<Articulo>();
                if (!lista.Any(a => a.Codigo == art.Codigo))
                {
                    lista.Add(art);
                    ListaArticulos.ItemsSource = null;
                    ListaArticulos.ItemsSource = lista;
                }
            }
        }

        private void cmbRubro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRubro.SelectedItem is string rubroNombre)
            {
                var rubro = Rubros.FirstOrDefault(r => r.Nombre == rubroNombre);
                if (rubro != null)
                {
                    cmbSubRubro.ItemsSource = rubro.SubRubros.Select(sr => sr.Nombre).ToList();
                    cmbSubRubro.SelectedIndex = 0;
                }
            }
        }

        private void cmbSubRubro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRubro.SelectedItem is string rubroNombre && cmbSubRubro.SelectedItem is string subRubroNombre)
            {
                var subRubro = Rubros
                    .FirstOrDefault(r => r.Nombre == rubroNombre)?
                    .SubRubros.FirstOrDefault(sr => sr.Nombre == subRubroNombre);

                if (subRubro != null)
                    ListaTareas.ItemsSource = subRubro.Tareas;
            }
        }
    }

    public class Articulo
    {
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public double Factor { get; set; }
        public string Moneda { get; set; }
        public double Precio { get; set; }
        public string Codigo { get; set; }
    }

    public class Tarea
    {
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public double Precio { get; set; }
        public string Rubro { get; set; }
        public string SubRubro { get; set; }
    }

    public class Rubro
    {
        public string Nombre { get; set; }
        public List<SubRubro> SubRubros { get; set; } = new();
    }

    public class SubRubro
    {
        public string Nombre { get; set; }
        public List<Tarea> Tareas { get; set; } = new();
    }
}
