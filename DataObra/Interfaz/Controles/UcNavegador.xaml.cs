using System.Windows;
using System.Windows.Controls;

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
                Width = 50,
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
        }
    }
}








