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

            _detalleCollection = new ObservableCollection<GastoDetalleDTO>();
            gridDetalle.ItemsSource = _detalleCollection;
            CrearRegistroDetalle();
        }

        public void CrearRegistroDetalle()
        {
            var nuevo = new GastoDetalleDTO
            {
                ID = 10,
                GastoID = 10,
                UsuarioID = 10,
                TipoID = 'M',
                Presupuesto = "Vivienda unif. calle 13",
                Descrip = "Cemento",
                Unidad = "Bolsa",
                Cantidad = 12,
                PrecioUnitario = 1000,
                Moneda = 'P',
            };

            _detalleCollection.Add(nuevo);

        }

    }
}
