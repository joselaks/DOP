using System;
using System.Windows;

namespace DataObra
{
    /// <summary>
    /// Lógica de interacción para DobleInicio.xaml
    /// </summary>
    public partial class DobleInicio : Window
    {
        public DobleInicio()
        {
            InitializeComponent();
        }

        private void Seba_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana Inicio.xaml
            Inicio inicioWindow = new Inicio();
            inicioWindow.Show();
            this.Close();
        }

        private void Jose_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana WiInicio.xaml
            Interfaz.Ventanas.WiInicio wiInicioWindow = new Interfaz.Ventanas.WiInicio();
            wiInicioWindow.Show();
            this.Close();
        }
    }
}
