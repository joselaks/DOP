using Biblioteca.DTO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DOP.Presupuestos.Ventanas
{
    public partial class WiNuevoPres : Window
    {
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;
        public WiNuevoPres(ObservableCollection<PresupuestoDTO> presupuestosRef)
        {
            InitializeComponent();
            _presupuestosRef = presupuestosRef;

            // Ejemplo: si tienes <StackPanel x:Name="panelModelos" ... /> en tu XAML
            // Usa panelModelos directamente, no crees un nuevo StackPanel aquí

            var opciones = new[]
            {
                new { Titulo = "Modelo 1", Descripcion = "Vivienda Unifamiliar", Parametro = "Modelo:Modelo 1" },
                new { Titulo = "Modelo 2", Descripcion = "Edificio de Propiedad Horizontal", Parametro = "Modelo:Modelo 2" },
                new { Titulo = "Modelo 3", Descripcion = "Descripción propia", Parametro = "Propio:Propio 1" },
                new { Titulo = "Modelo 4", Descripcion = "Descripción breve", Parametro = "Modelo:Modelo 1" },
                new { Titulo = "Modelo 5", Descripcion = "Otra descripción", Parametro = "Modelo:Modelo 2" },
                new { Titulo = "Modelo 6", Descripcion = "Descripción breve", Parametro = "Modelo:Modelo 1" },
                new { Titulo = "Modelo 7", Descripcion = "Otra descripción", Parametro = "Modelo:Modelo 2" },
                new { Titulo = "Propio 8", Descripcion = "Descripción propia", Parametro = "Propio:Propio 1" }
            };

            foreach (var opcion in opciones)
            {
                var boton = new Button
                    {
                    Margin = new Thickness(0, 0, 10, 10),
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    Style = (Style)FindResource("RoundedButtonStyle")
                    };
                boton.Focusable = false; // <--- Esto elimina el rectángulo celeste de foco

                // Crea el UserControl y asigna los textos
                var uc = new DOP.Presupuestos.Controles.UcModelo
                {
                    TituloTexto = opcion.Titulo,
                    DescripcionTexto = opcion.Descripcion
                };

                boton.Content = uc;

                // Evento click con confirmación
                boton.Click += (s, e) =>
                {
                    if (Confirmar($"¿Desea crear un presupuesto en base a {opcion.Titulo}?"))
                        AbrirPresupuesto(opcion.Parametro);
                };

                // Agrega el botón al StackPanel existente en tu XAML
                panelModelos.Children.Add(boton);
            }
        }

        private bool Confirmar(string mensaje)
        {
            return MessageBox.Show(mensaje, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private void AbrirPresupuesto(string parametro)
        {
            var win = new WiPresupuesto(null,null,null, _presupuestosRef);
            win.Owner = this.Owner;
            win.Show();
            this.Close();
        }

        private void btnVacio_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

