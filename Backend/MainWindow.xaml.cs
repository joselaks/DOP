using Backend.Datos;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Backend
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        public MainWindow()
            {
            InitializeComponent();
            

            }

        private async void VerificaUsuario()
            {

            int maxRetries = 3;

            for (int i = 0; i < maxRetries; i++)
                {
                DateTime startTime = DateTime.Now;
                var respuesta = await DatosWeb.ValidarUsuarioAsync("seba@dataobra.com", "contra");
                if (respuesta.Success)
                    {
                    if (respuesta.Usuario.DatosUsuario != null)
                        {
                        MessageBox.Show("Usuario encontrado");
                        return;

                        }
                    else
                        {
                        MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                        }
                    }
                else
                    {
                    if (i == maxRetries - 1)
                        {
                        var result = MessageBox.Show($"\n{respuesta.Message}\n¿Desea intentar nuevamente?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (result == MessageBoxResult.Yes)
                            {
                            VerificaUsuario();
                            }
                        return;
                        }
                    }
                }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VerificaUsuario();
        }
    }
    }