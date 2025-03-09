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
        Agrupador agrup;
        UcAgrupadores Origen;
        public UcAgrupador(Agrupador? Agrup, UcAgrupadores origen)
        {
            InitializeComponent();
            Origen = origen;
            if (Agrup != null)
            {
                agrup = Agrup;
                TipoIDTextBox.Text = agrup.TipoID.ToString();
                DescripcionTextBox.Text = agrup.Descrip;
                NumeroTextBox.Text = agrup.Numero.ToString();
                ActivoCheckBox.IsChecked = agrup.Active;

                NuevoEditar.Text = "Editar";
                edicion = true;

            }
            else
            {
                agrup = new Agrupador();
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
                agrup.TipoID = tipoID[0]; // Convertir el primer carácter de la cadena a char
                agrup.Descrip = descripcion;
                agrup.Editado = DateTime.Now;
                agrup.CuentaID = (short)App.IdCuenta; // Conversión explícita de int a short
                agrup.UsuarioID = App.IdUsuario;
                agrup.Active = ActivoCheckBox.IsChecked.Value;
                agrup.Numero = NumeroTextBox.Text;

                // Agregar la lógica para guardar el nuevo Agrupador
                if (edicion==false) 
                {
                    //var respuesta = await ConsultasAPI.PostAgrupadorAsync(agrup);

                    ////Respuestas
                    //int? nuevodoc = respuesta.Id;
                    //bool conexionExitosa = respuesta.Success;
                    //string mensaje = respuesta.Message;

                    ////Mensaje para testeo
                    //MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());
                    

                }
                else
                {
                    //var respuesta = await ConsultasAPI.PutAgrupadorAsync(agrup);
                    ////Respuestas
                    //bool conexionExitosa = respuesta.Success;
                    //string mensaje = respuesta.Message;
                    ////Mensaje para testeo
                    //MessageBox.Show(respuesta.Success + " " + mensaje);

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




