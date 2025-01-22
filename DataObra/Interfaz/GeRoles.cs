using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DataObra.Interfaz.Ventanas; // Asegúrate de que este espacio de nombres sea correcto
using DataObra.Interfaz.Controles;
using DataObra.Interfaz.Controles.SubControles;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using System.Windows.Media.Animation;
using Syncfusion.DocIO.DLS;

public class GeRoles
{
    public string Rol;
    public WiPrincipal principalWindow;

    public async void EjecutarProcedimiento(Button boton, Window window)
    {
        Rol = boton.Name;

        // Obtener el color de fondo del botón
        var backgroundBrush = boton.Background as SolidColorBrush;
        var backgroundColor = backgroundBrush?.Color;

        // Convertir el color a formato HEX
        string hexColor = backgroundColor.HasValue ? backgroundColor.Value.ToString() : "#005CA2";

        // Cerrar la ventana actual
        window.Close();

        await Task.Delay(2000);

        // Abrir la ventana WiPrincipal después de cerrar WiRoles
        principalWindow = new WiPrincipal(hexColor, boton.Name);

        //Configurancio la ventana principal de acuerdo al rol
        GeneraPanel();
        GeneraDocumentos();
        GeneraMateriales();
        GeneraInformes();

        principalWindow.Show();

        // Cerrar la ventana WiInicio (si es necesario)
        if (Application.Current.Windows.OfType<WiInicio>().FirstOrDefault() is WiInicio wiInicioWindow)
        {
            wiInicioWindow.Close();
        }
    }

    private void GeneraPanel()
    {
        UcPanelx4 ucPanelx4 = new UcPanelx4();
        principalWindow.contenidoPanel.Children.Add(ucPanelx4);
        switch (Rol)
        {
            case "BotonPresupuestos":

                //zona00
                UsGraficoBarras grafico1 = new UsGraficoBarras();
                // Crear un nuevo TabNavigationItem
                TabNavigationItem tabNavigation = new TabNavigationItem();
                tabNavigation.Header = "Incidencia de materiales";
                tabNavigation.Content = grafico1;
                ucPanelx4.zona00.Items.Add(tabNavigation);

                //zona01
                UsGrilla grilla1 = new UsGrilla();
                SfTreeGrid grilla = grilla1.grilla;
                TabItemExt tabItem = new TabItemExt();
                tabItem.Header = "Ultimos documentos editados";
                tabItem.Content = grilla1;
                var columns = new Dictionary<string, string>
                {
                    { "Tipo", "Documento" },
                    { "Nombre", "Nombre" },
                    { "ID", "ID" },
                    { "Fecha", "Fecha" },
                    { "Importe", "Importe" }
                };
                //Configura la grilla
                ConfiguraGrilla(columns, grilla);

                ucPanelx4.zona01.Items.Add(tabItem);

                //zona11
                // Crear un nuevo TabItemExt con 4 botones centrados
                TabItemExt tabItemConBotones = new TabItemExt();
                tabItemConBotones.Header = "Botones Centrados";
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center, // Especificar el espacio de nombres completo
                    VerticalAlignment = System.Windows.VerticalAlignment.Center // Especificar el espacio de nombres completo
                };
                var botones = new Dictionary<string, string>
                {
                    { "Presupuesto", "Presupuesto" },
                    { "Pedido", "Pedido" },
                    { "ID", "ID" },
                    { "Fecha", "Fecha" },
                    { "Importe", "Importe" }
                };

                agregaBoton(botones, stackPanel);

                tabItemConBotones.Content = stackPanel;
                ucPanelx4.zona11.Items.Add(tabItemConBotones);
                break;
            default:
                break;
        }
    }

    private void GeneraInformes()
    {
    }

    private void GeneraMateriales()
    {
    }

    private void GeneraDocumentos()
    {
    }

    private void agregaBoton(Dictionary<string, string> botones, StackPanel contenedor)
    {
        foreach (var item in botones)
        {
            Button button = new Button
            {
                Width = 100,
                Height = 100,
                Content = item.Key,
                Name = item.Value,
                Margin = new Thickness(5),
                Style = (System.Windows.Style)principalWindow.FindResource("RoundedRadioButtonStyle")
            };
            button.Click += principalWindow.Boton_Click;
            contenedor.Children.Add(button);
        }
    }

    private void ConfiguraGrilla(Dictionary<string, string> campos, SfTreeGrid grilla)
    {
        foreach (var column in campos)
        {
            TreeGridTextColumn textColumn = new TreeGridTextColumn
            {
                MappingName = column.Key,
                HeaderText = column.Value
            };

            grilla.Columns.Add(textColumn);
        }
    }
}
