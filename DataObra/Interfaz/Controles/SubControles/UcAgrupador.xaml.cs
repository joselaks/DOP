using Biblioteca.DTO;
using DataObra.Datos;
using Syncfusion.Windows.Controls;
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

            CargarTipos();

            if (Agrup != null)
            {
                agrupador = Agrup;
                //TipoIDTextBox.Text = agrupador.TipoID.ToString();
                Dispatcher.Invoke(() =>
                {
                    TipoComboBox.SelectedValue = agrupador.TipoID;
                }, System.Windows.Threading.DispatcherPriority.Loaded);


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

        private void CargarTipos()
        {
            var tipos = new Dictionary<char, string>
            {
            { 'O', "Obra" },
            { 'A', "Administración" },
            { 'C', "Cliente" },
            { 'P', "Proveedor" },
            { 'E', "Empleado" },
            { 'S', "SubContratista" },
            { 'U', "Cuenta" },
            { 'D', "Deposito" },
            { 'I', "Impuesto" },
            { 'T', "Tema" }
        };
            TipoComboBox.ItemsSource = tipos;
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            
            string descripcion = DescripcionTextBox.Text;

            if (agrupador.TipoID == '\0' || descripcion.Length == 0)
            {
                MessageBox.Show("Por favor, complete todos los campos y asegúrese de que TipoID sea válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                agrupador.TipoID = agrupador.TipoID; // Convertir el primer carácter de la cadena a char
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

        private void TipoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TipoComboBox.SelectedValue != null)
            {
                agrupador.TipoID = (char)TipoComboBox.SelectedValue;
            }
        }
    }
}




