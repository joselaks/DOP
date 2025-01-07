using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using Syncfusion.Windows.Tools.Controls;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiPrincipal.xaml
    /// </summary>
    public partial class WiPrincipal : Window, INotifyPropertyChanged
    {
        private SolidColorBrush _primaryColorBrush;

        public SolidColorBrush PrimaryColorBrush
        {
            get { return _primaryColorBrush; }
            set
            {
                _primaryColorBrush = value;
                OnPropertyChanged(nameof(PrimaryColorBrush));
            }
        }

        string Rol;

        public WiPrincipal(string hexColor, string rol)
        {
            InitializeComponent();
            Rol = rol;
            DataContext = this;
            PrimaryColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
            CargaTabs();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CargaTabs()
        {
            Interfaz.Controles.UcNavegador ucNavegador1 = new Controles.UcNavegador(Rol, "Documentos");
            contenidoDocumentos.Children.Add(ucNavegador1);
            Interfaz.Controles.UcNavegador ucNavegador2 = new Controles.UcNavegador(Rol, "Agrupadores");
            contenidoAgrupadores.Children.Add(ucNavegador2);
            Controles.UcPanelx4 ucPanelx4 = new Controles.UcPanelx4();
            contenidoPanel.Children.Add(ucPanelx4);

            Controles.SubControles.UsGraficoBarras grafico1 = new Controles.SubControles.UsGraficoBarras();
            Controles.SubControles.UsGrilla grilla1 = new Controles.SubControles.UsGrilla();

            // Crear un nuevo TabNavigationItem
            TabNavigationItem tabNavigation = new TabNavigationItem();
            tabNavigation.Header = "Incidencia de materiales";
            tabNavigation.Content = grafico1;
            ucPanelx4.zona00.Items.Add(tabNavigation);

            TabItemExt tabItem = new TabItemExt();
            tabItem.Header = "Ultimos documentos editados";
            tabItem.Content = grilla1;
            ucPanelx4.zona01.Items.Add(tabItem);

            // Crear un nuevo TabItemExt con 4 botones centrados
            TabItemExt tabItemConBotones = new TabItemExt();
            tabItemConBotones.Header = "Botones Centrados";

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Definir el estilo del botón con bordes redondeados
            Style roundedButtonStyle = new Style(typeof(Button));
            roundedButtonStyle.Setters.Add(new Setter(Button.WidthProperty, 100.0));
            roundedButtonStyle.Setters.Add(new Setter(Button.HeightProperty, 100.0));
            roundedButtonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(10)));
            roundedButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
            roundedButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Colors.Gray)));
            roundedButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));

            ControlTemplate template = new ControlTemplate(typeof(Button));
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(25));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            roundedButtonStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

            for (int i = 1; i <= 4; i++)
            {
                Button button = new Button
                {
                    Style = roundedButtonStyle
                };

                // Crear un StackPanel para contener la imagen y el texto
                StackPanel buttonContent = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Agregar la imagen al StackPanel
                Image icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/DataObra;component/Interfaz/Imagenes/Mas.png")),
                    Width = 30,
                    Height = 30
                };
                buttonContent.Children.Add(icon);

                // Agregar el texto al StackPanel
                TextBlock text = new TextBlock
                {
                    Text = "documento",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                buttonContent.Children.Add(text);

                // Establecer el StackPanel como el contenido del botón
                button.Content = buttonContent;
                stackPanel.Children.Add(button);
            }

            tabItemConBotones.Content = stackPanel;
            ucPanelx4.zona11.Items.Add(tabItemConBotones);
        }




        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Maximize_Click(sender, e);
            }
            else
            {
                DragMove();
            }
        }

        private void hRol_Click(object sender, RoutedEventArgs e)
        {
            // Preguntar al usuario si quiere ejecutar la orden
            var result = MessageBox.Show("¿Está seguro de que quiere reingresar al sistema con un nuevo rol?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Reiniciar la aplicación
                var currentProcess = Process.GetCurrentProcess();
                Process.Start(currentProcess.MainModule.FileName);
                Application.Current.Shutdown();
            }
        }
    }
}









