using Biblioteca.DTO;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos.Ventanas;
using DOP;
using Syncfusion.UI.Xaml.Charts;
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


namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xModelos.xaml
    /// </summary>
    public partial class xModelos : UserControl
    {
        private WiEscritorio escritorio;
        private ObservableCollection<PresupuestoDTO> _modelos;


        public xModelos(WiEscritorio _escritorio, ObservableCollection<PresupuestoDTO> modelos)
        {
            InitializeComponent();
            escritorio = _escritorio;
            _modelos = modelos;
            // Botones para modelos
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

                var uc = new DOP.Presupuestos.Controles.UcModelo
                    {
                    TituloTexto = modelo.Descrip,
                    CostoTotal = modelo.PrEjecTotal.ToString("N2"),
                    Superficie = modelo.Superficie.HasValue ? modelo.Superficie.Value.ToString("N2") : "",
                    ValorM2 = modelo.ValorM2.ToString("N2"),
                    };

                boton.Content = uc;

                boton.MouseDoubleClick += async (s, e) =>
                {
                    // Deshabilita todos los botones
                    foreach (Button b in PanelModelos.Children.OfType<Button>())
                        b.IsEnabled = false;

                    CrearCopia(modelo);
                };

                PanelModelos.Children.Add(boton);
                }



            this.Loaded += XModelos_Loaded;

            }

        private void XModelos_Loaded(object sender, RoutedEventArgs e)
            {



            }

        private bool Confirmar(string mensaje)
            {
            return MessageBox.Show(mensaje, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }

        private async void CrearCopia(PresupuestoDTO _modelosSeleccionado)
            {
            var (ok, msg, conceptos, relaciones) = await DOP.Datos.DatosWeb.ObtenerConceptosYRelacionesAsync(_modelosSeleccionado.ID.Value);
            if (ok)
                {
                if (conceptos != null)
                    foreach (var c in conceptos)
                        c.PresupuestoID = 0;
                if (relaciones != null)
                    foreach (var r in relaciones)
                        r.PresupuestoID = 0;

                var copia = PresupuestoDTO.CopiarPresupuestoDTO(_modelosSeleccionado);


                copia.ID = 0; // que el ID sea cero para un nuevo presupuesto
                copia.UsuarioID = App.IdUsuario; // el ID del usuario actual
                copia.EsModelo = false; // que no sea un modelo

                var wiPresupuesto = new WiPres(copia, conceptos, relaciones, null);

                // Suscríbete al evento Closed para habilitar los botones
                wiPresupuesto.Closed += (s, e) =>
                {
                    foreach (Button b in PanelModelos.Children.OfType<Button>())
                        b.IsEnabled = true;
                };

                wiPresupuesto.ShowDialog();
                }
            else
                {
                MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        private void Button_Click(object sender, RoutedEventArgs e)
            {
            escritorio.CambioEstado("mModelo", "Normal", "M");
            }
        
    }
}
