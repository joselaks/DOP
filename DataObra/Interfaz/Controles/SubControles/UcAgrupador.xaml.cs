using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Controles.SubControles
{
    public partial class UcAgrupador : UserControl
    {
        bool edicion;
        AgrupadorDTO agrupador;
        UcAgrupadores Origen;

        public UcAgrupador(AgrupadorDTO? Agrup, UcAgrupadores origen)
        {
            InitializeComponent();
            Origen = origen;

            var tipos = new Dictionary<char, string>
            {
                { 'P', "Proveedor" },
                { 'O', "Obra" },
                { 'A', "Administración" },
                { 'C', "Cliente" }
            };

            TipoComboBox.ItemsSource = tipos;

            if (Agrup != null)
            {
                agrupador = Agrup;
                //TipoIDTextBox.Text = agrupador.TipoID.ToString();
                TipoComboBox.SelectedValue = agrupador.TipoID;
                DescripcionTextBox.Text = agrupador.Descrip;
                NumeroTextBox.Text = agrupador.Numero.ToString();
                ActivoCheckBox.IsChecked = agrupador.Active;

                NuevoEditar.Text = "Editar";
                edicion = true;
            }
            else
            {
                agrupador = new AgrupadorDTO();
                ActivoCheckBox.IsChecked = true;

                NuevoEditar.Text = "Nuevo";
                edicion = false;
            }
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string tipoID = TipoIDTextBox.Text;
            string descripcion = DescripcionTextBox.Text;

            if (string.IsNullOrEmpty(tipoID) || tipoID.Length != 1 || string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Por favor, complete todos los campos y asegúrese de que TipoID sea un solo carácter.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                agrupador.TipoID = tipoID[0]; // Convertir el primer carácter de la cadena a char
                agrupador.Descrip = descripcion;
                agrupador.Editado = DateTime.Now;
                agrupador.CuentaID = (short)App.IdCuenta; // Conversión explícita de int a short
                agrupador.UsuarioID = App.IdUsuario;
                agrupador.Active = ActivoCheckBox.IsChecked.Value;
                agrupador.Numero = NumeroTextBox.Text;

                if (edicion)
                {
                    var (success, message) = await DatosWeb.ActualizarAgrupadorAsync(agrupador);

                    // Manejar la respuesta
                    if (success)
                    {
                        MessageBox.Show($"Agrupador actualizado con éxito. ID: {agrupador.ID}  Descripción: {agrupador.Descrip}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Error al actualizar el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Llamar al método InsertarAgrupadorAsync
                    var (success, message, id) = await DatosWeb.InsertarAgrupadorAsync(agrupador);

                    // Manejar la respuesta
                    if (success && id.HasValue)
                        MessageBox.Show($"Agrupador creado con éxito. ID: {id.Value}  Descripción: {agrupador.Descrip}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show($"Error al crear el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Origen.CargarGrilla();

                // Cerrar la ventana que contiene este UserControl
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.Close();
                }
            }
        }
    }
}




