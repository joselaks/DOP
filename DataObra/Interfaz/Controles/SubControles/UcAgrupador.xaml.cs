using DataObra.Agrupadores;
using DataObra.Datos;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Controles.SubControles
{
    public partial class UcAgrupador : UserControl
    {
        Agrupador agrup;
        public UcAgrupador()
        {
            InitializeComponent();
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string tipoID = TipoIDTextBox.Text;
            string nombre = NombreTextBox.Text;
            string descripcion = DescripcionTextBox.Text;
            DateTime fechaCreacion = DateTime.Now;

            if (string.IsNullOrEmpty(tipoID) || tipoID.Length != 1 || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Por favor, complete todos los campos y asegúrese de que TipoID sea un solo carácter.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                agrup = new Agrupador();
                agrup.TipoID = tipoID[0]; // Convertir el primer carácter de la cadena a char
                agrup.Descrip = descripcion;
                agrup.Editado = fechaCreacion;
                agrup.CuentaID = (short)App.IdCuenta; // Conversión explícita de int a short
                agrup.UsuarioID = App.IdUsuario;

                // Agregar la lógica para guardar el nuevo Agrupador
                var respuesta = await ConsultasAPI.PostAgrupadorAsync(agrup);

                //Respuestas
                int? nuevodoc = respuesta.Id;
                bool conexionExitosa = respuesta.Success;
                string mensaje = respuesta.Message;

                //Mensaje para testeo
                MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());

                MessageBox.Show($"Agrupador guardado exitosamente.\nFecha de Creación: {fechaCreacion}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

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




