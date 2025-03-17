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

            if (Agrup != null)
            {
                agrupador = Agrup;
                TipoIDTextBox.Text = agrupador.TipoID.ToString();
                DescripcionTextBox.Text = agrupador.Descrip;
                NumeroTextBox.Text = agrupador.Numero.ToString();
                ActivoCheckBox.IsChecked = agrupador.Active;

                NuevoEditar.Text = "Editar";
                edicion = true;

            }
            else
            {
                agrupador = new AgrupadorDTO();

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
                    // Llamar al método InsertarAgrupadorAsync
                    var (success, message, id) = await DatosWeb.InsertarAgrupadorAsync(agrupador);

                    // Manejar la respuesta
                    if (success && id.HasValue)
                        MessageBox.Show($"Agrupador creado con éxito. ID: {id.Value}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show($"Error al crear el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {

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




