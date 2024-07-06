using DataObra.Documentos.Clases;
using DataObra.Sistema.Clases;
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

        private void GrillaAgrupadores_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Abre el Agrupador con todos los documentos relacionados

            switch (Tipo)
            {
                case "Obras":
                    string[] colObra = { "Presupuestos", "Planes", "Certificados", "Partes", "Remitos", "Facturas" };
                    foreach (var item in colObra)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                case "Clientes":
                    string[] colAdmin = { "Facturas", "Cobros" };
                    foreach (var item in colAdmin)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                case "Proveedores":
                    string[] colProv = { "Compras", "Remitos", "Facturas", "Pagos" };
                    foreach (var item in colProv)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                case "Contratistas":
                    string[] colCont = { "Contratos", "Remitos", "Facturas", "Pagos" };
                    foreach (var item in colCont)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                case "Obreros":
                    string[] colObrero = { "Partes", "Sueldo", "Pagos" };
                    foreach (var item in colObrero)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                case "Admin":
                    string[] colTec = { "Acopios", "Pedidos", "Compras", "Remitos", "Facturas", "Pagos" };
                    foreach (var item in colTec)
                    {
                        CreaCarpetas(item);
                    }
                    break;
                default:
                    break;
            }
        }

        public void CreaCarpetas(string pAgrupador)
        {
            // Crea instancia de TileView
            TileViewControl Tiles = new TileViewControl();

            // Items
            TileViewItem tileView = new TileViewItem();
            tileView.Width = 400;
            tileView.Height = 350;
            tileView.Margin = new Thickness(5);
            tileView.Header = pAgrupador;
            tileView.Content = "Documetos de " + pAgrupador;

            // Agregar el TileViewItem al TileViewControl
            Tiles.Items.Add(tileView);

            Grilla.Children.Add(Tiles);
        }

        //private void CreaCarpetas(string item)
        //{
        //    RadTileViewItem carpeta = new RadTileViewItem();
        //    carpeta.Style = (Style)FindResource("RadTileViewItemStyle");
        //    carpeta.Header = item;

        //    switch (item)
        //    {
        //        case "Presupuestos":
        //            cListadoPresupuestos listapre = new cListadoPresupuestos("1");
        //            carpeta.Content = listapre;
        //            break;
        //        case "Facturas":
        //            break;
        //    }

        //    navegador.Items.Add(carpeta);
        //}

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

