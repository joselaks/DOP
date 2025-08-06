using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        int listaActiva = 0; // Variable para almacenar el ID de la lista activa

        public UcArticulos()
            {
            InitializeComponent();
            InfoCombo = new ObservableCollection<ListaArticuloItem>();
            this.DataContext = this; // Asegura el binding
            }

        private void SelectorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            var item = SelectorTipo.SelectedItem as ListaArticuloItem;
            if (item != null)
                {
                listaActiva = item.ID;
                // Usar el idSeleccionado según lo necesites
                }

            }

        private async void obtener_Click(object sender, RoutedEventArgs e)
            {
            InfoCombo.Clear(); // Limpia la colección vinculada

            int usuarioID = App.IdUsuario;
            var (success, message, listas) = await DOP.Datos.DatosWeb.ObtenerListasArticulosPorUsuarioAsync(usuarioID);

            if (!success)
                {
                MessageBox.Show($"No se pudo obtener las listas: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            foreach (var lista in listas)
                {
                InfoCombo.Add(new ListaArticuloItem
                    {
                    ID = lista.ID,
                    Descrip = lista.Descrip
                    });
                }
            }

        private async void articulos_Click(object sender, RoutedEventArgs e)
            {
            if (listaActiva == 0)
                {
                MessageBox.Show("Seleccione una lista de precios antes de continuar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            var (success, message, articulos) = await DOP.Datos.DatosWeb.ObtenerArticulosPorListaIDAsync((short)listaActiva);

            if (!success)
                {
                MessageBox.Show($"No se pudieron obtener los artículos: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            // Asignar los datos a la grilla
            grillaPrecios.ItemsSource = articulos.Select(a => new
                {
                Codigo = a.Codigo,
                Descripcion = a.Descrip,
                Unidad = a.Unidad,
                PU1 = a.Precio
                }).ToList();
            }


        public class ListaArticuloItem
            {
            public int ID { get; set; }
            public string Descrip { get; set; }
            }

        public ObservableCollection<ListaArticuloItem> InfoCombo
            {
            get;
            set;
            }

        }
    }
