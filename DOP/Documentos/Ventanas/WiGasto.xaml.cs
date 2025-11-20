using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DOP;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace DataObra.Documentos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiGasto.xaml
    /// </summary>
    public partial class WiGasto : RibbonWindow
        {
        // Colección que alimenta gridDetalle
        public Gasto objeto;
        public ObservableCollection<GastoDTO> gastos = new();
        // Guardar la referencia del encabezado original pasado al constructor
        private GastoDTO? originalEncabezado;


        public WiGasto(ObservableCollection<GastoDTO> _gastos, GastoDTO encabezado, List<GastoDetalleDTO> detalle, bool tipoGasto)
            {
            InitializeComponent();
            objeto = new Gasto(encabezado, detalle, tipoGasto);
            this.DataContext = objeto;
            gastos = _gastos;
            // Vincular el encabezado (GastoDTO) como DataContext de la grilla de encabezado
            if (grillaEncabezado != null)
                grillaEncabezado.DataContext = objeto.encabezado;

            gridDetalle.ItemsSource = objeto.detalleGrabar;
            }


        private void AgregarRegistro_Click(object sender, RoutedEventArgs e)
            {
            // Crear un nuevo detalle con valores por defecto mínimos
            var nuevo = new GastoDetalleDTO
                {
                ID = 0,
                TipoID= 'M',
                Cantidad = 0,
                PrecioUnitario = 0,
                Importe = 0,
                Descrip = string.Empty
                };

            // Asegurar la lista y añadir
            if (objeto.detalleGrabar == null)
                objeto.detalleGrabar = new List<GastoDetalleDTO>();

            objeto.detalleGrabar.Add(nuevo);

            // Re-enlazar ItemsSource para que la grilla actualice (detalleGrabar es List<T>)
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = objeto.detalleGrabar;

            // Seleccionar el nuevo registro para que el usuario pueda editarlo inmediatamente
            gridDetalle.SelectedItem = nuevo;
            gridDetalle.Focus();
            }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {
            // Ejecuta el guardado asincrónico y cierra/indica resultado igual que en WiPres
            if (await GuardarGastoAsync())
                {
                MessageBox.Show("Gasto guardado correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                try
                    {
                    this.DialogResult = true;
                    }
                catch (InvalidOperationException)
                    {
                    // No fue abierto como diálogo; seguir
                    }
                this.Close();
                }
            }

        private void BtnSalirSinGuardar_Click(object sender, RoutedEventArgs e)
            {
            try
                {
                this.DialogResult = false;
                }
            catch (InvalidOperationException)
                {
                }
            this.Close();
            }

        private async Task<bool> GuardarGastoAsync()
            {
            // Actualizar totales y marcas temporales
            try
                {
                // Actualizar importe total desde detalleGrabar
                if (objeto?.detalleGrabar != null)
                    {
                    objeto.encabezado.Importe = objeto.detalleGrabar.Sum(d => d?.Importe ?? 0);
                    }

                objeto.encabezado.FechaEditado = DateTime.Now;

                // Asegurar UsuarioID si existe App.IdUsuario en la app (como WiPres)
                try
                    {
                    objeto.encabezado.UsuarioID = App.IdUsuario;
                    }
                catch
                    {
                    // Si no existe App.IdUsuario no hacemos nada
                    }

                // Empaquetar request usando la lógica de Gasto (compara detalleLeer y detalleGrabar)
                Biblioteca.DTO.ProcesarGastoRequest oGrabar = objeto.EmpaquetarGasto();

                // Llamada al servicio web para procesar el gasto
                // Llamada al servicio web para procesar el gasto
                var procesarTaskResult = await DOP.Datos.DatosWeb.ProcesarGastoAsync(oGrabar);
                var success = procesarTaskResult.Success;
                var message = procesarTaskResult.Message;
                var procesaResult = procesarTaskResult.Result;

                if (success)
                    {
                    // Actualiza listas para próxima ejecución (sincronizar versiones)
                    objeto.detalleLeer = objeto.detalleGrabar.Select(x => x).ToList();

                    // Si fue nuevo, asignar ID y FechaC/FechaCreado si la API devolvió DocumentoID
                    if ((objeto.encabezado.ID == 0) && (procesaResult?.DocumentoID ?? 0) > 0)
                        {
                        objeto.encabezado.FechaCreado = DateTime.Today;
                        objeto.encabezado.ID = procesaResult.DocumentoID;
                        }

                    // Reemplazar en la colección 'gastos' la instancia original por la instancia actualizada
                    if (gastos != null)
                        {
                        int idx = -1;
                        // Primero intentar por referencia (la instancia que se pasó al constructor)
                        if (originalEncabezado != null)
                            idx = gastos.IndexOf(originalEncabezado);

                        // Si no está por referencia, intentar localizar por ID (después de haber podido asignar DocumentoID)
                        if (idx == -1 && objeto.encabezado.ID != 0)
                            idx = gastos.ToList().FindIndex(g => g.ID == objeto.encabezado.ID);

                        if (idx >= 0)
                            {
                            // Reemplazo: la colección recibe la nueva instancia (objeto.encabezado)
                            gastos[idx] = objeto.encabezado;
                            }
                        else
                            {
                            // Si no existía en la colección, añadirla
                            gastos.Add(objeto.encabezado);
                            }
                        }

                    var escritorio = Application.Current.Windows
                                    .OfType<DataObra.Interfaz.Ventanas.WiEscritorio>()
                                    .FirstOrDefault();

                    if (escritorio != null && procesaResult != null)
                        {
                        // Ejecutar en el hilo de UI del escritorio
                        escritorio.Dispatcher.Invoke(() => escritorio.UpdatePresupuestosFromGastoResult(procesaResult));
                        }

                    return true;
                    }
                else
                    {
                    var msg = string.IsNullOrEmpty(message) ? "Error desconocido al guardar el gasto." : message;
                    MessageBox.Show($"Error al guardar el gasto: {msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Excepción al guardar el gasto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                }
            }

        }

    public class ProcesarGastoRequest
        {
        public GastoDTO Gasto { get; set; }
        public List<GastoDetalleDTO> Detalles { get; set; } = new List<GastoDetalleDTO>();
        public List<int> PresupuestosAfectados { get; set; } = new List<int>();
        }
    }