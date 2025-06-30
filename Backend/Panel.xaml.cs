using Backend.Datos;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biblioteca.DTO;



namespace Backend
{
    public partial class Panel : Window
    {
        private List<Insumo> Insumos = new();
        private List<Articulo> Articulos = new();
        private List<Tarea> Tareas = new();
        private List<Articulo> ArticulosProveedor = new();
        private Dictionary<string, List<string>> InsumoArticuloMap = new();
        private Dictionary<string, List<string>> TareaInsumoMap = new();
        private Dictionary<string, List<string>> RubroSubRubroMap = new();
        private Dictionary<(string Rubro, string SubRubro), List<Tarea>> RubroSubRubroTareasMap = new();
        private List<Rubro> Rubros = new();

        Conectores meconecto = new Conectores();

        public Panel()
        {
            InitializeComponent();

            //CargarDatosSimulados();  
            _ = CargarInsumos();

            lstInsumos.ItemsSource = Insumos;
            lstTareas.ItemsSource = Tareas;
            lstArticulosProveedor.ItemsSource = ArticulosProveedor;
        }

        private async Task CargarInsumos()
        {
            int usuarioID = 1;
            var (success1, message1, insumos) = await meconecto.ObtenerInsumosPorUsuarioAsync(usuarioID);
            if (success1)
            {
                foreach (var insumo in insumos)
                    Console.WriteLine($"{insumo.ID} - {insumo.Descrip}");
            }
            else
            {
                MessageBox.Show("Error al buscar insumos");
                //Console.WriteLine($"Error: {message1}");
            }
        }
        private void CargarDatosSimulados()
        {
            Insumos = DatosSimulados.ObtenerInsumos();
            ArticulosProveedor = DatosSimulados.ObtenerArticulosProveedor();
            Tareas = DatosSimulados.ObtenerTareas();
            InsumoArticuloMap = DatosSimulados.ObtenerInsumoArticuloMap();
            TareaInsumoMap = DatosSimulados.ObtenerTareaInsumoMap();

            lstInsumos.ItemsSource = Insumos;
            lstTareas.ItemsSource = Tareas;
            lstArticulosProveedor.ItemsSource = ArticulosProveedor;

            Articulos.AddRange(ArticulosProveedor.Where(a => a.Codigo == "ART001" || a.Codigo == "ART003"));
            lstArticulos.ItemsSource = Articulos;

            Rubros = DatosSimulados.ObtenerRubrosConSubrubros();
            cmbRubro.ItemsSource = Rubros.Select(r => r.Nombre).ToList();

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

        private void cmbRubro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRubro.SelectedItem is string rubroNombre)
            {
                var rubro = Rubros.FirstOrDefault(r => r.Nombre == rubroNombre);
                if (rubro is not null)
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

                if (subRubro is not null)
                    lstTareas.ItemsSource = subRubro.Tareas;
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
