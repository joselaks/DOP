using DataObra.Agrupadores;
using DataObra.Agrupadores.Clases;
using DataObra.Documentos;
using DataObra.Insumos;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DataObra.Sistema
{
    public partial class Principal : Window
    {
        public Tiles listaAgrupadores;
        public Tiles listaDocumentos;
        public Tiles listaInsumos;
        public string selContenido;

        public Principal()
        {
            InitializeComponent();
            VerificarUsuario();
            PreseleccionarPanel();
        }

        private void VerificarUsuario()
        {
            if (string.IsNullOrWhiteSpace(UsuarioTexto.Text) || UsuarioTexto.Text == "Usuario: NombreUsuario")
            {
                Login loginWindow = new Login();
                if (loginWindow.ShowDialog() == true)
                {
                    ActualizarUsuario(loginWindow.Usuario);
                    ActualizarRol(loginWindow.Rol);
                }
                else
                {
                    // Si el login falla, cerrar la aplicación.
                    Application.Current.Shutdown();
                }
            }
            else
            {
                VerificarRol();
            }
        }

        private void VerificarRol()
        {
            if (string.IsNullOrWhiteSpace(RolTexto.Text) || RolTexto.Text == "NombreRol" || RolTexto.Text == "Demo")
            {
                SeleccionRol seleccionRolWindow = new SeleccionRol
                {
                    ParentWindow = this
                };
                seleccionRolWindow.ShowDialog();
            }
        }

        private void PreseleccionarPanel()
        {
            Boton_Click(BotonPanel, null);
        }

        private void Boton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botonPresionado)
            {
                TilesDocumentos.Visibility = Visibility.Collapsed;
                TilesAgrupadores.Visibility = Visibility.Collapsed;
                TilesInsumos.Visibility = Visibility.Collapsed;
                Panel.Visibility = Visibility.Collapsed;

                // Restaurar el color de fondo de todos los botones
                BotonPanel.Background = (SolidColorBrush)Resources["PanelBackground"];
                BotonAgrupadores.Background = (SolidColorBrush)Resources["AgrupadoresBackground"];
                BotonDocumentos.Background = (SolidColorBrush)Resources["DocumentosBackground"];
                BotonInsumos.Background = (SolidColorBrush)Resources["InsumosBackground"];

                // Cambiar el color del botón presionado
                botonPresionado.Background = (SolidColorBrush)Resources["PressedBackground"];

                // Actualizar el contenido
                string contenido = botonPresionado.Content.ToString();
                

                if (contenido != null)
                {
                    selContenido = contenido;

                    switch (contenido)
                    {
                        case "Documentos":
                            if (listaDocumentos == null)
                            {
                                listaDocumentos = new Tiles();
                                listaDocumentos.ListaString = new string[] { "Precostos", "Presupuestos", "Pedidos", "Planes", "Certificados", "Partes", "Facturas", "Compras", "Remitos", "Pagos", "Cobros", "Anticipos", "Contratos", "Sueldos", "Gastos", "Acopios" };

                                foreach (var item in listaDocumentos.ListaString)
                                {
                                    TilesDocumentos.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Turco"));
                                }
                            }
                            TilesDocumentos.Visibility = Visibility.Visible;
                            break;
                        case "Agrupadores":
                            if (listaAgrupadores == null)
                            {
                                listaAgrupadores = new Tiles();
                                listaAgrupadores.ListaString = new string[] { "Obras", "Admin", "Clientes", "Proveedores", "Contratistas", "Obreros", "Cuentas", "Impuestos", "Depositos", "Temas" };

                                foreach (string item in listaAgrupadores.ListaString)
                                {
                                    TilesAgrupadores.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Naranja"));
                                }
                            }
                            TilesAgrupadores.Visibility = Visibility.Visible;
                            break;
                        case "Insumos":
                            if (listaInsumos == null)
                            {
                                listaInsumos = new Tiles();
                                listaInsumos.ListaString = new string[] { "Materiales", "ManoDeObra", "Equipos", "SubContratos", "Otros", "Auxiliares", "Tareas", "Rubros", "Indices" };

                                foreach (var item in listaInsumos.ListaString)
                                {
                                    TilesInsumos.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Verde"));
                                }
                            }
                            TilesInsumos.Visibility = Visibility.Visible;
                            break;
                        default:
                            Panel.Visibility = Visibility.Visible;
                            break;
                }
                }
            }
        }


        private TileViewItem CrearTile(string pNombre, string pDescrip, string pColor)
        {
            TileViewItem nuevo = new TileViewItem();

            TileChico contenido = new TileChico(pNombre, pDescrip);
            nuevo.Content = contenido;

            nuevo.Name = pNombre;
            nuevo.MinMaxButtonVisibility = Visibility.Collapsed;
            nuevo.HeaderVisibility = Visibility.Collapsed;
            nuevo.MouseLeftButtonUp += ItemTile_Click;

            switch (pColor)
            {
                case "Naranja":
                    nuevo.Background = new SolidColorBrush(Colors.DarkOrange);
                    break;
                case "Violeta":
                    nuevo.Background = new SolidColorBrush(Colors.Violet);
                    break;
                case "Verde":
                    nuevo.Background = new SolidColorBrush(Colors.Green);
                    break;
                case "Azul":
                    nuevo.Background = new SolidColorBrush(Colors.Blue);
                    break;
                case "Turqueza":
                default:
                    nuevo.Background = new SolidColorBrush(Colors.Turquoise);
                    break;
            }

            return nuevo;
        }


        private void ItemTile_Click(object sender, MouseButtonEventArgs e)
        {
            TileViewItem tileSele = sender as TileViewItem;
            bool abierto = false;

            if (tileSele != null)
            {
                switch (selContenido)
                {
                    case "Documentos":
                        //NavDocumento navegador = new NavDocumento("", "");

                        break;
                    case "Agrupadores":
                        break;
                    case "Insumos":
                        break;
                    default:
                        break;
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            SeleccionRol seleccionRolWindow = new SeleccionRol
            {
                ParentWindow = this
            };
            seleccionRolWindow.ShowDialog();
        }

        // Métodos para actualizar la barra de estado
        public void ActualizarUsuario(string usuario)
        {
            UsuarioTexto.Text = $"Usuario: {usuario}";
        }

        public void ActualizarRol(string rol)
        {
            RolTexto.Text = rol;
            VerificarRol();
        }
    }
}
