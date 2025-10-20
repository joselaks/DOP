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
                GastoID = objeto?.encabezado?.ID ?? 0,
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

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {
            try
                {
                }
            catch (InvalidOperationException)
                {
                }
            this.Close();
            }

        private void BtnSalirSinGuardar_Click(object sender, RoutedEventArgs e)
            {
            try
                {
                }
            catch (InvalidOperationException)
                {
                }
            this.Close();
            }
        }
    }

