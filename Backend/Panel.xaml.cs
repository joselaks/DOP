using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Backend
{
    public partial class Panel : Window
    {
        private List<Insumo> Insumos = new();
        private List<Articulo> Articulos = new();
        private List<Tarea> Tareas = new();
        private List<Articulo> ArticulosProveedor = new();

        public Panel()
        {
            InitializeComponent();
            CargarDatosSimulados();
            lstInsumos.ItemsSource = Insumos;
            lstTareas.ItemsSource = Tareas;
            lstArticulosProveedor.ItemsSource = ArticulosProveedor;
        }

        private void CargarDatosSimulados()
        {
            Insumos.AddRange(new[] {
                new Insumo { Descripcion = "Cemento", Tipo = "Materiales", PrecioUnitario = 1200, Unidad = "KG", MetodoCalculo = "Directo", Codigo = "MAT001" },
                new Insumo { Descripcion = "Pintura", Tipo = "Materiales", PrecioUnitario = 850, Unidad = "LT", MetodoCalculo = "Promedio", Codigo = "MAT002" },
                new Insumo { Descripcion = "Albañil", Tipo = "Mano de Obra", PrecioUnitario = 3500, Unidad = "HS", MetodoCalculo = "Estimado", Codigo = "MO001" },
                new Insumo { Descripcion = "Excavadora", Tipo = "Equipos", PrecioUnitario = 8000, Unidad = "HS", MetodoCalculo = "Tarifa", Codigo = "EQ001" },
            });

            ArticulosProveedor.AddRange(new[] {
                new Articulo { Descripcion = "Bolsa Cemento Loma Negra", Unidad = "KG", Factor = 50, Moneda = "AR", Precio = 1200, Codigo = "ART001" },
                new Articulo { Descripcion = "Pintura Blanca 10L", Unidad = "LT", Factor = 10, Moneda = "AR", Precio = 8500, Codigo = "ART002" },
                new Articulo { Descripcion = "Pala de Punta", Unidad = "UN", Factor = 1, Moneda = "AR", Precio = 1500, Codigo = "ART003" },
                new Articulo { Descripcion = "Camión Mixer", Unidad = "HS", Factor = 1, Moneda = "AR", Precio = 10000, Codigo = "ART004" },
            });

            Tareas.AddRange(new[] {
                new Tarea { Descripcion = "Revoque Fino Interior", Unidad = "M2", Precio = 800 },
                new Tarea { Descripcion = "Colocación Cerámicos", Unidad = "M2", Precio = 1500 },
                new Tarea { Descripcion = "Pintura Interior", Unidad = "M2", Precio = 1200 },
                new Tarea { Descripcion = "Excavación de Zanjas", Unidad = "ML", Precio = 2500 },
            });

            Articulos.AddRange(new[] {
                new Articulo { Descripcion = "Bolsa Cemento Loma Negra", Unidad = "KG", Factor = 50, Moneda = "ARS", Precio = 1200, Codigo = "ART001" },
                new Articulo { Descripcion = "Pala de Punta", Unidad = "UN", Factor = 1, Moneda = "ARS", Precio = 1500, Codigo = "ART003" }
            });

            lstArticulos.ItemsSource = Articulos;
        }

        private void TxtBuscarInsumo_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarInsumos();
        }

        private void FiltrarInsumos()
        {
            string filtroTexto = txtBuscarInsumo.Text.ToLower();
            string filtroTipo = ((ComboBoxItem)cmbTipoInsumo.SelectedItem)?.Content?.ToString();

            var filtrado = Insumos.Where(i =>
                (filtroTipo == "Todos" || i.Tipo == filtroTipo) &&
                i.Descripcion.ToLower().Contains(filtroTexto)
            ).ToList();

            lstInsumos.ItemsSource = filtrado;
        }

        private void TxtBuscarTarea_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarTarea.Text.ToLower();
            lstTareas.ItemsSource = Tareas.Where(t => t.Descripcion.ToLower().Contains(filtro)).ToList();
        }

        private void TxtBuscarArticuloProveedor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarArticuloProveedor.Text.ToLower();
            lstArticulosProveedor.ItemsSource = ArticulosProveedor.Where(a => a.Descripcion.ToLower().Contains(filtro)).ToList();
        }

        private void BtnActualizarArticulos_Click(object sender, RoutedEventArgs e)
        {
            if (lstInsumos.SelectedItem is Insumo insumo)
            {
                lstArticulos.ItemsSource = ArticulosProveedor.Where(a => a.Descripcion.ToLower().Contains(insumo.Descripcion.ToLower())).ToList();
            }
        }

        private void BtnActualizarInsumosDesdeTarea_Click(object sender, RoutedEventArgs e)
        {
            if (lstTareas.SelectedItem is Tarea tarea)
            {
                lstInsumos.ItemsSource = Insumos.Where(i => i.Descripcion.ToLower().Contains(tarea.Descripcion.ToLower())).ToList();
            }
        }

        private void BtnAgregarArticuloDesdeProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (lstArticulosProveedor.SelectedItem is Articulo art)
            {
                AgregarArticulo(art);
            }
        }

        private void LstArticulosProveedor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is Articulo articulo)
            {
                DragDrop.DoDragDrop(listView, articulo, DragDropEffects.Copy);
            }
        }

        private void LstArticulos_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Articulo)))
            {
                var articulo = (Articulo)e.Data.GetData(typeof(Articulo));
                AgregarArticulo(articulo);
            }
        }

        private void LstArticulos_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Articulo)))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void AgregarArticulo(Articulo articulo)
        {
            var lista = lstArticulos.ItemsSource as List<Articulo> ?? new List<Articulo>();
            if (!lista.Any(a => a.Codigo == articulo.Codigo))
            {
                lista.Add(articulo);
                lstArticulos.ItemsSource = null;
                lstArticulos.ItemsSource = lista;
            }
        }
    }

    public class Insumo
    {
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public double PrecioUnitario { get; set; }
        public string Unidad { get; set; }
        public string MetodoCalculo { get; set; }
        public string Codigo { get; set; }
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
    }
}
