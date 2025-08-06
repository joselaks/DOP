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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcArticulos.xaml
    /// </summary>
    public partial class UcArticulos : UserControl
    {
        public UcArticulos()
        {
            InitializeComponent();
        }

        private void SelectorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }

        private async void obtener_Click(object sender, RoutedEventArgs e)
            {
            // Limpia el ComboBox antes de cargar nuevos datos
            SelectorTipo.Items.Clear();

            // Obtiene el ID del usuario (ajusta según tu contexto)
            int usuarioID = App.IdUsuario;

            // Llama al método para obtener las listas de artículos
            var (success, message, listas) = await DOP.Datos.DatosWeb.ObtenerListasArticulosPorUsuarioAsync(usuarioID);

            if (!success)
                {
                MessageBox.Show($"No se pudo obtener las listas: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            // Agrega cada lista al ComboBox con ID y Descrip
            foreach (var lista in listas)
                {
                SelectorTipo.Items.Add(new
                    {
                    lista.ID,
                    lista.Descrip
                    });
                }

            // Opcional: selecciona el primer elemento si hay datos
            if (SelectorTipo.Items.Count > 0)
                SelectorTipo.SelectedIndex = 0;
            }

        private void articulos_Click(object sender, RoutedEventArgs e)
            {

            }
        }
}
