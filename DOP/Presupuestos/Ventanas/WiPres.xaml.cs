using Bibioteca.Clases;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DOP;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
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

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiPres.xaml
    /// </summary>
    public partial class WiPres : ChromelessWindow
        {
        public Presupuesto Objeto;
        public cUndoRedo UndoRedo;
        public UcPlanilla Planilla;
        public UcListado Listado;
        public UcDosaje Dosaje;
        public UcMaestro Maestro;
        public UcArticulos Articulos;
        public UcPlanillaListado PlanillaListado;
        public UcMaestroPrecios MaestroPrecios;
        private ContenedorPresupuesto _contenedor;
        private ObservableCollection<PresupuestoDTO> _presupuestosRef;

        public WiPres(PresupuestoDTO? _encabezado, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, ObservableCollection<PresupuestoDTO> presupuestosRef)
            {
            InitializeComponent();
            Objeto = new Presupuesto(_encabezado, conceptos, relaciones);
            Objeto.encabezado.UsuarioID = App.IdUsuario;
            Dosaje = new UcDosaje(Objeto);
            Planilla = new UcPlanilla(Objeto, Dosaje);
            Listado = new UcListado(Objeto);
            Maestro = new UcMaestro();
            Articulos = new UcArticulos();
            PlanillaListado = new UcPlanillaListado();
            MaestroPrecios = new UcMaestroPrecios();
            _contenedor = new ContenedorPresupuesto();
            this.gridPres.Children.Add(_contenedor);
            this.Loaded += WiPres_Loaded;
            this.Closing += WiPres_Closing;
            _presupuestosRef = presupuestosRef;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth <= 1920 && screenHeight <= 1080)
            {
                // Maximizar si la resolución es igual o menor a 1920x1080

                WindowState = WindowState.Maximized;


            }
            else
            {
                // Centrar y establecer tamaño fijo si la resolución es mayor

                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowState = WindowState.Normal;
                Width = 1900;
                Height = 1030;
            }

        }

        private void WiPres_Closing(object? sender, CancelEventArgs e)
        {
            // Ejemplo: Confirmar cierre
            var result = MessageBox.Show("¿Está seguro que desea cerrar el presupuesto?", "Confirmar cierre", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true; // Cancela el cierre
            }

            // Aquí puedes agregar lógica adicional, como guardar cambios, liberar recursos, etc.
        }

        private void WiPres_Loaded(object sender, RoutedEventArgs e)
        {
            _contenedor.gridPlanilla.Children.Add(Planilla);
            _contenedor.gridListado.Children.Add(Listado);
            _contenedor.gridDetalle.Children.Add(Dosaje); 
            gridMaestro.Children.Add(Maestro);
        }

        private void ventanas_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // vDetalle: filas 2 y 3 de basePres
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItemDetalle && menuItemDetalle.Name == "vDetalle")
            {
                if (_contenedor.basePres.RowDefinitions.Count >= 3)
                {
                    if (menuItemDetalle.IsChecked == true)
                    {
                        _contenedor.basePres.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                        _contenedor.basePres.RowDefinitions[2].Height = new GridLength(300);
                    }
                    else
                    {
                        _contenedor.basePres.RowDefinitions[1].Height = new GridLength(0);
                        _contenedor.basePres.RowDefinitions[2].Height = new GridLength(0);
                    }
                }
            }

            // vListado: columnas 2 y 3 de basePres
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItemListado && menuItemListado.Name == "vListado")
            {
                if (_contenedor.basePres.ColumnDefinitions.Count >= 3)
                {
                    if (menuItemListado.IsChecked == true)
                    {
                        _contenedor.basePres.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        _contenedor.basePres.ColumnDefinitions[2].Width = new GridLength(300);
                    }
                    else
                    {
                        _contenedor.basePres.ColumnDefinitions[1].Width = new GridLength(0);
                        _contenedor.basePres.ColumnDefinitions[2].Width = new GridLength(0);
                    }
                }
            }

            // vMaestro: columnas 1 y 2 de gridPres
            if (d is Syncfusion.Windows.Tools.Controls.DropDownMenuItem menuItemMaestro && menuItemMaestro.Name == "vMaestro")
            {
                if (gridBase.ColumnDefinitions.Count >= 2)
                {
                    if (menuItemMaestro.IsChecked == true)
                    {
                        gridBase.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        gridBase.ColumnDefinitions[2].Width = new GridLength(600);
                    }
                    else
                    {
                        gridBase.ColumnDefinitions[1].Width = new GridLength(0);
                        gridBase.ColumnDefinitions[2].Width = new GridLength(0);
                    }
                }
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

            Objeto.encabezado.PrEjecTotal = Objeto.Arbol.Sum(i => i.Importe1);
            Objeto.encabezado.FechaM = DateTime.Now;

            ProcesaPresupuestoRequest oGrabar = Objeto.EmpaquetarPresupuesto();

            // Llamada al servicio web para procesar el presupuesto
            var resultado = await DOP.Datos.DatosWeb.ProcesarPresupuestoAsync(oGrabar);

            if (resultado.Success)
            {
                // --- ACTUALIZACIÓN DE LISTAS PARA PRÓXIMA EJECUCIÓN ---
                Objeto.listaConceptosLeer = Objeto.listaConceptosGrabar.Select(x => x).ToList();
                Objeto.listaRelacionesLeer = Objeto.listaRelacionesGrabar.Select(x => x).ToList();

                MessageBox.Show($"Presupuesto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);


                if (Objeto.encabezado.ID == null || Objeto.encabezado.ID == 0)
                {
                    // Nuevo presupuesto: asignar fecha de creación y ID, luego agregar a la colección
                    Objeto.encabezado.FechaC = DateTime.Today;
                    Objeto.encabezado.ID = resultado.PresupuestoID;
                    var superficie = Objeto.encabezado.Superficie ?? 0;
                    if (Objeto.encabezado.Superficie.HasValue && Objeto.encabezado.Superficie.Value > 0)
                        Objeto.encabezado.ValorM2 = Math.Round(Objeto.encabezado.PrEjecTotal / Objeto.encabezado.Superficie.Value, 2);
                    else
                        Objeto.encabezado.ValorM2 = 0;

                    _presupuestosRef.Add(PresupuestoDTO.CopiarPresupuestoDTO(Objeto.encabezado));
                }
                else
                {
                    // Edición: buscar y reemplazar en la colección
                    var existente = _presupuestosRef.FirstOrDefault(p => p.ID == Objeto.encabezado.ID);
                    if (existente != null)
                    {
                        // Copia los valores de Objeto.encabezado (la copia) al objeto original
                        existente.Descrip = Objeto.encabezado.Descrip;
                        existente.FechaC = Objeto.encabezado.FechaC;
                        existente.FechaM = Objeto.encabezado.FechaM;
                        existente.MesBase = Objeto.encabezado.MesBase;
                        existente.PrEjecTotal = Objeto.encabezado.PrEjecTotal;
                        existente.Superficie = Objeto.encabezado.Superficie;
                        existente.EsModelo = Objeto.encabezado.EsModelo;

                        // Recalcular ValorM2 de forma segura
                        if (existente.Superficie.HasValue && existente.Superficie.Value > 0)
                            existente.ValorM2 = Math.Round(existente.PrEjecTotal / existente.Superficie.Value, 2);
                        else
                            existente.ValorM2 = 0;



                        // ... y el resto de propiedades
                    }
                    else
                    {
                        // Si no se encuentra, lo agrega (caso raro)
                        _presupuestosRef.Add(Objeto.encabezado);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Error al guardar el presupuesto: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFiebdc_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Archivo Fiebdc|*.bc3";

            if (openFileDialog.ShowDialog().Value)
            {
                FileStream stream = File.OpenRead(openFileDialog.FileName);
                string textoFie;
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    textoFie = reader.ReadToEnd();
                    //this.txtArchivoActualiza.Text = "Archivo seleccionado";
                    string txtNombre = stream.Name;
                }
                Bibioteca.Clases.Fiebdc fie = new Bibioteca.Clases.Fiebdc(textoFie);
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto(null, null, null);

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto(null, null, null);
                objetofieb.generaPresupuesto("fie", fie.listaConceptos, fie.listaRelaciones);
                foreach (var item in objetofieb.Arbol)
                {
                    Objeto.Arbol.Add(item);
                }
                Objeto.RecalculoCompleto();
            }

        }

        private void SaleExcel_Click(object sender, RoutedEventArgs e)
        {
            // 1. Crear el Excel con el presupuesto actual
            using (var excel = new DOP.Presupuestos.Clases.Excel(Objeto))
            {
                // 2. Generar la hoja que desees (puedes cambiar el método)
                excel.PresupuestoEjecutivo();
                //excel.PresupuestoTipos(); // Genera la hoja de tipos de presupuesto
                // También podrías usar: excel.PresupuestoTipos(); o excel.PresupuestoComercial();

                // 3. Guardar el archivo temporalmente
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Presupuesto_{Guid.NewGuid():N}.xlsx");
                excel.book.SaveAs(tempPath);

                // 4. Intentar abrir el archivo con Excel
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    // Si no se pudo abrir, permitir al usuario elegir dónde guardar el archivo
                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Title = "Guardar archivo Excel",
                        Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
                        FileName = "Presupuesto.xlsx"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        try
                        {
                            File.Copy(tempPath, saveDialog.FileName, true);
                            MessageBox.Show($"Archivo guardado en:\n{saveDialog.FileName}", "Guardado exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception copyEx)
                        {
                            MessageBox.Show($"No se pudo guardar el archivo.\nError: {copyEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"No se pudo abrir Excel automáticamente.\nArchivo temporal generado en:\n{tempPath}\n\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WiDatosGenerales(Objeto.encabezado)
            {
                Owner = this
            };
            if (dlg.ShowDialog() == true)
            {
                // Los cambios ya están reflejados en Objeto.encabezado por el binding
                // Si necesitas notificar cambios manualmente, hazlo aquí
            }
        }



    }
}
