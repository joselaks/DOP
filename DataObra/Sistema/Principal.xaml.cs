using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DataObra.Sistema
{
    public partial class Principal : Window
    {
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
                // Restaurar el color de fondo de todos los botones
                BotonPanel.Background = (SolidColorBrush)Resources["PanelBackground"];
                BotonAgrupadores.Background = (SolidColorBrush)Resources["AgrupadoresBackground"];
                BotonDocumentos.Background = (SolidColorBrush)Resources["DocumentosBackground"];
                BotonInsumos.Background = (SolidColorBrush)Resources["InsumosBackground"];

                // Cambiar el color del botón presionado
                botonPresionado.Background = (SolidColorBrush)Resources["PressedBackground"];

                // Actualizar el contenido
                string contenido = botonPresionado.Content.ToString();
                AreaContenido.Content = new TextBlock { Text = contenido, FontSize = 24, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
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
