using DataObra.Interfaz.Controles.SubControles;
using DataObra.Sistema;
using Syncfusion.UI.Xaml.TreeGrid;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace DataObra.Interfaz.Controles
{
    public partial class UcNavegador : UserControl
    {
        string Rol;
        string Tab;

        public UcNavegador(string rol, string tab)
        {
            InitializeComponent();
            Rol = rol;
            Tab = tab;
            CrearBotones();
            llenaGrilla();
        }

        private void llenaGrilla()
        {
            grillaArbol.Columns.Add(new TreeGridTextColumn() { MappingName = "Tipo", HeaderText = "Tipo de documento" });
            grillaArbol.Columns.Add(new TreeGridTextColumn() { MappingName = "Nombre", HeaderText = "Nombre" });
            grillaArbol.Columns.Add(new TreeGridTextColumn() { MappingName = "ID", HeaderText = "ID" });
            grillaArbol.Columns.Add(new TreeGridDateTimeColumn() { MappingName = "Fecha" });
            grillaArbol.Columns.Add(new TreeGridNumericColumn() { MappingName = "Importe" });
        }

        private void CrearBotones()
        {
            switch (Rol)
            {
                case "BotonPresupuestos":
                    if (Tab == "Documentos")
                    {
                        var ListaString = new string[] { "Presupuestos", "Planos", "Planes", "Comparativas" };

                        foreach (string item in ListaString)
                        {
                            agregaBoton(item);
                        }
                    }
                    if (Tab == "Agrupadores")
                    {
                        var ListaString = new string[] { "Obras", "Clientes" };

                        foreach (string item in ListaString)
                        {
                            agregaBoton(item);
                        }
                    }
                    break;

                case "BotonCompras":
                    if (Tab == "Documentos")
                    {
                        var ListaString = new string[] { "Pedidos", "Ordenes de compra", "Remitos", "Facturas" };

                        foreach (string item in ListaString)
                        {
                            agregaBoton(item);
                        }
                    }
                    if (Tab == "Agrupadores")
                    {
                        var ListaString = new string[] { "Obras", "Clientes", "Proveedores" };

                        foreach (string item in ListaString)
                        {
                            agregaBoton(item);
                        }
                    }
                    break;
                default:
                    for (int i = 0; i < 5; i++)
                    {
                        agregaBoton("Boton " + i);  
                    }


                    break;
            }
           
        }

        private void agregaBoton(string item)
        {
            RadioButton radioButton = new RadioButton
            {
                Width = 100,
                Height = 35,
                Content = item,
                Margin = new Thickness(5),
                Style = (Style)FindResource("RoundedRadioButtonStyle")
            };
            radioButton.Checked += RadioButton_Checked;
            items.Children.Add(radioButton);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            MessageBox.Show($"RadioButton {radioButton.Content} seleccionado");

            switch (radioButton.Content)
            {
                case "Obras":

                    break;
                default:
                    break;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataObra.Interfaz.Ventanas.WiDocumento ventana = new DataObra.Interfaz.Ventanas.WiDocumento();
            var mainWindow = Window.GetWindow(this);

            // Aplicar efecto de desenfoque a la ventana principal
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            //// Calcular el nuevo tamaño y posición de la ventana modal
            //ventana.Width = mainWindow.ActualWidth - 20;
            //ventana.Height = mainWindow.ActualHeight - 20;
            //ventana.Left = mainWindow.Left + 10;
            //ventana.Top = mainWindow.Top + 10;

            // Mostrar la ventana de manera modal
            ventana.ShowDialog();

            // Quitar el efecto de desenfoque después de cerrar la ventana modal
            mainWindow.Effect = null;
        }
    }
}








