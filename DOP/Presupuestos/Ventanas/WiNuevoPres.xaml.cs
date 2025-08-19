using Bibioteca.Clases;
using Biblioteca.DTO;
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DOP.Presupuestos.Ventanas
    {
    public partial class WiNuevoPres : Window
        {
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;
        private ObservableCollection<PresupuestoDTO> _modelos;


        public WiNuevoPres(ObservableCollection<PresupuestoDTO> presupuestosRef, ObservableCollection<PresupuestoDTO> modelos, ObservableCollection<PresupuestoDTO> modelosPropios)
            {
            InitializeComponent();
            _presupuestosRef = presupuestosRef;
            _modelos = modelos;
            // Genera los botones dinámicamente a partir de _modelos
            foreach (var modelo in _modelos)
                {
                var boton = new Button
                    {
                    Margin = new Thickness(0, 0, 10, 10),
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    Style = (Style)FindResource("RoundedButtonStyle")
                    };
                boton.Focusable = false;

                // Crea el UserControl y asigna los textos desde el modelo
                var uc = new DOP.Presupuestos.Controles.UcModelo
                    {
                    TituloTexto = modelo.Descrip,
                    CostoTotal = modelo.PrEjecTotal.ToString("N2"),
                    Superficie = modelo.Superficie.HasValue ? modelo.Superficie.Value.ToString("N2") : "",
                    ValorM2 = modelo.ValorM2.ToString("N2"),
                    };

                boton.Content = uc;

                // Evento click con confirmación y paso del modelo seleccionado
                boton.Click += async (s, e) =>
                {
                    if (Confirmar($"¿Desea crear un presupuesto en base a {modelo.Descrip}?"))
                        CrearCopia(modelo);
                };

                panelModelos.Children.Add(boton);
                //panelModelosPropios.Children.Add(boton1); // Agrega también a modelos propios
                }
            }

        private bool Confirmar(string mensaje)
            {
            return MessageBox.Show(mensaje, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }

        private async void CrearCopia(PresupuestoDTO _modelosSeleccionado)
            {
            // Tengo que crear una copia del modelo, con ID de presupuesto en cero e ID de usuario


            var (ok, msg, conceptos, relaciones) = await Datos.DatosWeb.ObtenerConceptosYRelacionesAsync(_modelosSeleccionado.ID.Value);
            if (ok)
                {
                //Asigna PresupuestoID = 0 a todos los conceptos y relaciones
                if (conceptos != null)
                    foreach (var c in conceptos)
                        c.PresupuestoID = 0;
                if (relaciones != null)
                    foreach (var r in relaciones)
                        r.PresupuestoID = 0;

                _modelosSeleccionado.ID = 0; // Asegúrate de que el ID sea cero para un nuevo presupuesto
                _modelosSeleccionado.UsuarioID = App.IdUsuario; // Asigna el ID del usuario actual
                _modelosSeleccionado.Descrip = $"{_modelosSeleccionado.Descrip} - Copia"; // Modifica la descripción para indicar que es una copia
                _modelosSeleccionado.EsModelo = false; // Asegúrate de que no sea un modelo

                // Aquí puedes pasar conceptos y relaciones a la ventana WiPresupuesto si lo necesitas
                var copia = PresupuestoDTO.CopiarPresupuestoDTO(_modelosSeleccionado);
                var wiPresupuesto = new WiPresupuesto(copia, conceptos, relaciones, _presupuestosRef);
                wiPresupuesto.Owner = this;
                wiPresupuesto.ShowDialog();
                // Si necesitas usar conceptos y relaciones después, puedes hacerlo aquí
                }
            else
                {
                MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            this.Close();
            }

        private void btnVacio_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiPresupuesto(null, null, null, _presupuestosRef);
            win.Owner = this.Owner;
            win.Show();
            this.Close();

            }

        private void Button_Click(object sender, RoutedEventArgs e)
            {

            }
        }
    }

