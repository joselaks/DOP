using Bibioteca.Clases;
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
        private ObservableCollection<GastoDetalleDTO> _detalleCollection;

        public WiGasto()
        {
            InitializeComponent();

            // Inicializar colección y asignarla como ItemsSource de la grilla
            _detalleCollection = new ObservableCollection<GastoDetalleDTO>();
            gridDetalle.ItemsSource = _detalleCollection;

            // Registro de prueba para verificar que la grilla muestre elementos.
            // Elimina o comenta esta línea cuando todo funcione y añadas registros desde UI/servicio.
            CrearRegistroDetalle('M', "Artículo de prueba", "u", 2.5m, 150.75m, presupuestoID: 1001, insumoID: "INS-001", rubroID: "R-01", tareaID: "T-01");
        }

        /// <summary>
        /// Crea un nuevo registro de detalle y lo añade a gridDetalle.
        /// Si se pasan valores se usan; si no, se crean valores por defecto.
        /// </summary>
        public GastoDetalleDTO CrearRegistroDetalle(
            char tipoID = ' ',
            string? descrip = null,
            string? unidad = null,
            decimal cantidad = 1m,
            decimal precioUnitario = 0m,
            int? presupuestoID = null,
            string? insumoID = null,
            string? rubroID = null,
            string? tareaID = null)
        {
            var nuevo = new GastoDetalleDTO
            {
                ID = 0,
                GastoID = 0,
                UsuarioID = 0,
                TipoID = tipoID,
                PresupuestoID = presupuestoID,
                RubroID = rubroID,
                TareaID = tareaID,
                AuxiliarID = null,
                InsumoID = insumoID,
                Descrip = descrip,
                Unidad = unidad,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario,
                Moneda = ' ',
                Importe = cantidad * precioUnitario,
                Accion = ' '
            };

            _detalleCollection.Add(nuevo);

            try
            {
                // Seleccionar el nuevo registro en la grilla
                gridDetalle.SelectedItem = nuevo;

                // Intentar desplazar la vista al registro nuevo usando APIs que pueden existir
                // en diferentes versiones de SfDataGrid. Se hace en dynamic para evitar errores
                // de compilación si el método no está definido.
                try
                {
                    dynamic dg = gridDetalle;
                    // varios nombres usados en distintas versiones: ScrollInToView / ScrollIntoView / BringIntoView
                    try { dg.ScrollInToView(nuevo); } catch { }
                    try { dg.ScrollIntoView(nuevo); } catch { }
                    try { dg.BringIntoView(); } catch { }
                }
                catch
                {
                    // si la conversión dinámica falla, seguimos sin fallo
                }

                // Enfocar la grilla para permitir edición inmediata
                gridDetalle.Focus();
            }
            catch
            {
                // ignorar cualquier excepción de selección/scroll para no romper la creación
            }

            return nuevo;
        }

        /// <summary>
        /// Actualiza el Importe de un detalle (Cantidad * PrecioUnitario).
        /// Llamar tras editar campos numéricos.
        /// </summary>
        public void RecalcularImporte(GastoDetalleDTO detalle)
        {
            if (detalle == null) return;

            detalle.Importe = detalle.Cantidad * detalle.PrecioUnitario;

            // FORMA SEGURA SIN INotifyPropertyChanged: reemplazamos el elemento en la colección
            var idx = _detalleCollection.IndexOf(detalle);
            if (idx >= 0)
            {
                var copia = new GastoDetalleDTO
                {
                    ID = detalle.ID,
                    GastoID = detalle.GastoID,
                    UsuarioID = detalle.UsuarioID,
                    TipoID = detalle.TipoID,
                    PresupuestoID = detalle.PresupuestoID,
                    RubroID = detalle.RubroID,
                    TareaID = detalle.TareaID,
                    AuxiliarID = detalle.AuxiliarID,
                    InsumoID = detalle.InsumoID,
                    Descrip = detalle.Descrip,
                    Unidad = detalle.Unidad,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Moneda = detalle.Moneda,
                    Importe = detalle.Importe,
                    Accion = detalle.Accion
                };

                _detalleCollection[idx] = copia;
                gridDetalle.SelectedItem = copia;
            }
        }
    }
}
