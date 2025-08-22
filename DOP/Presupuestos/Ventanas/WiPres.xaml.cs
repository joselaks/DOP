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
    }
    }
