using System.Windows;
using System.Windows.Controls;

namespace DataObra.Sistema
{
    public partial class SeleccionRol : Window
    {
        public Principal ParentWindow { get; set; }

        public SeleccionRol()
        {
            InitializeComponent();
        }

        private void Boton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botonPresionado)
            {
                string contenido = botonPresionado.Content.ToString();

                // Enviar el rol seleccionado a la ventana principal
                if (ParentWindow != null)
                {
                    ParentWindow.ActualizarRol(contenido);
                }

                // Cerrar la ventana de SeleccionRol
                this.Close();
            }
        }
    }
}
