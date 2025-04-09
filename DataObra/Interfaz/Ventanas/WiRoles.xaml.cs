using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiRoles.xaml
    /// </summary>
    public partial class WiRoles : Window
    {
        private Window _inicio;

        public WiRoles(Window inicio)
        {
            InitializeComponent();
            _inicio = inicio;
        }

        private void Boton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton)
            {
                // Configurar el SfBusyIndicator como activo
                if (_inicio is WiInicio inicioWindow)
                {
                    inicioWindow.espera.IsBusy = true;
                    inicioWindow.espera.Header = "Configurando su espacio de trabajo......";
                }

                // Crear una instancia de GeRoles y llamar al procedimiento
                GeRoles geRoles = new GeRoles();
                geRoles.EjecutarProcedimiento(boton, this);
                App.Rol = boton.Name;
            }
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
           
                
                Application.Current.Shutdown();
           
        }
    }
}











