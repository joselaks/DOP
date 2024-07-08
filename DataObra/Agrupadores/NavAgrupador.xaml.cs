using DataObra.Documentos;
using Syncfusion.ProjIO;
using Syncfusion.UI.Xaml.Kanban;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.Windows.Shared;
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
using Syncfusion.Windows.Controls.Layout;
using Syncfusion.UI.Xaml.Diagram.Stencil;
using DataObra.Sistema;

namespace DataObra.Agrupadores.Clases
{
    public partial class NavAgrupador : UserControl
    {
        Servidor azure;
        public ObservableCollection<KanbanModel> Listado { get; set; }
        private KanbanModel selectedItem;

        int TipoAgrupa;
        string Tipo;

        public NavAgrupador(string pTipo)
        {
            InitializeComponent();
            
            Tipo = pTipo;

            switch (pTipo)
            {
                case "Obras":
                    TipoAgrupa = 1;
                    break;
                case "Admin":
                    TipoAgrupa = 2;
                    break;
                case "Clientes":
                    TipoAgrupa = 3;
                    break;
                default:
                    TipoAgrupa = 100;
                    break;
            }

            CargaAgrupadores(TipoAgrupa);
        }

        private void CargaAgrupadores(int pTipo)
        {
            azure = new Servidor();

            Listado = new ObservableCollection<KanbanModel>();
            KanbanModel task;

            foreach (var item in azure.Agrupadores.Where(a => a.TipoID == TipoAgrupa))
            {
                task = new KanbanModel();

                task.ID = item.ID.ToString();
                task.Title = item.Descrip;
                task.Description = item.Numero.ToString();

                if (item.Active)
                {
                    task.Category = "Activos";
                }
                else
                {
                    task.Category = "Pendientes";
                }

                Listado.Add(task);
            }

            this.GrillaAgrupadores.ItemsSource = null;
            this.GrillaAgrupadores.ItemsSource = Listado;
        }
        private void GrillaAgrupadores_CardTapped(object sender, KanbanTappedEventArgs e)
        {
            selectedItem = e.SelectedCard.Content as KanbanModel;
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            KanbanModel task = new KanbanModel();

            task.Title = "Nuevo Agrupador";
            task.Description = "Nuevo";
            task.Category = "Pendientes";

            Listado.Add(task);
        }
        private void AbrirAgrupador()
        {
            // Mapeo de tipos a colecciones de documentos
            var documentosPorTipo = new Dictionary<string, string[]>
            {
                ["Obras"] = new[] { "Presupuestos", "Planes", "Certificados", "Partes", "Remitos", "Facturas" },
                ["Clientes"] = new[] { "Presupuestos", "Facturas", "Cobros" },
                ["Proveedores"] = new[] { "Acopios", "Compras", "Remitos", "Facturas", "Pagos" },
                ["Contratistas"] = new[] { "Contratos", "Remitos", "Facturas", "Pagos" },
                ["Obreros"] = new[] { "Partes", "Sueldo", "Pagos" },
                ["Admin"] = new[] { "Acopios", "Pedidos", "Compras", "Remitos", "Facturas", "Pagos" },
                ["Cuentas"] = new[] { "Ingresos", "Egresos"},
                ["Depositos"] = new[] { "Entradas", "Salidas"},
                ["Impuestos"] = new[] { "Iva", "IB", "Ganancias"},
                ["Temas"] = new[] { "Pendientes", "En Proceso", "Terminados" }
            };

            TileViewControl Tiles = new TileViewControl();

            if (documentosPorTipo.TryGetValue(Tipo, out var documentos))
            {
                foreach (var item in documentos)
                {
                    var tileView = CrearTileViewItem(item, item + " de " + selectedItem.Title);
                    Tiles.Items.Add(tileView);
                }
            }

            Grilla.Children.Clear();
            Grilla.Children.Add(Tiles);
        }

        private void GrillaAgrupadores_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            AbrirAgrupador();
        }

        private void AbrirAgrupador_Click(object sender, RoutedEventArgs e)
        {
            AbrirAgrupador();
        }

        private TileViewItem CrearTileViewItem(string header, string content)
        {
            return new TileViewItem
            {
                Width = 800,
                Height = 750,
                Margin = new Thickness(5),
                Header = header,
                Content = content
            };
        }

        private void EditarFicha_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                var sele = azure.Agrupadores.FirstOrDefault(a => a.ID == selectedItem.ID.ConvertToInt64Null());

                if (sele != null)
                {
                    FichaAgrupador fichaWindow = new FichaAgrupador(sele, 1);
                    fichaWindow.AgrupadorModified += FichaWindow_AgrupadorModified;
                    fichaWindow.Show();
                }
            }
        }

        private void FichaWindow_AgrupadorModified(object? sender, Agrupador e)
        {
            var modificado = Listado.FirstOrDefault(a => a.ID == e.ID.ToString());

            if (modificado != null)
            {
                // Actualiza las propiedades del objeto existente
                modificado.Title = e.Descrip;
                modificado.Description = e.Numero.ToString();
            }
        }

    }
}

