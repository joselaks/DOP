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

        public WiGasto(GastoDTO encabezado, List<GastoDetalleDTO> detalle)
            {
            InitializeComponent();
            objeto = new Gasto(encabezado, detalle);
            this.DataContext = objeto;
            gridDetalle.ItemsSource = objeto.detalleGrabar;
            }


        }

    }

