using Biblioteca.DTO;
using System;
using System.Windows;

namespace DOP.Presupuestos.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiNuevaListaArticulos.xaml
    /// </summary>
    public partial class WiNuevaListaArticulos : Window
    {
        public ArticulosListaDTO NuevaLista { get; private set; }

        public WiNuevaListaArticulos()
        {
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescrip.Text))
            {
                MessageBox.Show("Debe ingresar una descripción.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tipoId = txtTipoID.Text?.Trim().ToUpper();
            string[] valoresValidos = { "M", "D", "O", "S", "E" };
            if (Array.IndexOf(valoresValidos, tipoId) == -1)
            {
                MessageBox.Show("El tipo debe artículo está vacío", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NuevaLista = new ArticulosListaDTO
            {
                CuentaID = 1, // Puedes ajustar según tu lógica
                UsuarioID = 4,
                TipoID = tipoId,
                EntidadID = null,
                Descrip = txtDescrip.Text,
                Fecha = DateTime.Now,
                Moneda = "P",
                Active = true
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
