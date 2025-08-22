using Bibioteca.Clases;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DOP;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Shapes;

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiPres.xaml
    /// </summary>
    public partial class WiPres : RibbonWindow
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
                        gridBase.ColumnDefinitions[2].Width = new GridLength(300);
                    }
                    else
                    {
                        gridBase.ColumnDefinitions[1].Width = new GridLength(0);
                        gridBase.ColumnDefinitions[2].Width = new GridLength(0);
                    }
                }
            }
        }




    }
}
