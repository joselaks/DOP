using DataObra.Documentos;
using DataObra.Sistema;
using Syncfusion.UI.Xaml.Kanban;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DataObra.Agrupadores.Clases
{
    public partial class NavAgrupador : UserControl
    {
        #region Declaraciones
        Servidor azure;
        public ObservableCollection<KanbanModel> Listado { get; set; }
        private KanbanModel selectedItem;

        int TipoAgrupa;
        string Tipo;
        #endregion
        TabItemExt TabItemAbierto;
        TabControl TabControlAbierto;

        public NavAgrupador(string pTipo, TabItemExt pTab, TabControlExt pTabControl)
        {
            InitializeComponent();
            TabItemAbierto = pTab;
            TabControlAbierto = pTabControl;

            #region PRINCIPALES
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

        private void AbrirAgrupador()
        {
            var esta = TabControlAbierto.Items.Cast<TabItemExt>().FirstOrDefault(t => t.Header.ToString() == selectedItem.Title);

            if (esta == null)
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
                    ["Cuentas"] = new[] { "Ingresos", "Egresos" },
                    ["Depositos"] = new[] { "Entradas", "Salidas" },
                    ["Impuestos"] = new[] { "Iva", "IB", "Ganancias" },
                    ["Temas"] = new[] { "Pendientes", "En Proceso", "Terminados" }
                };

                TileViewControl Tiles = new TileViewControl();

                if (documentosPorTipo.TryGetValue(Tipo, out var documentos))
                {
                    bool primero = true;

                    foreach (var item in documentos)
                    {
                        var tileView = CrearTileViewItem(item, item + " de " + selectedItem.Title, primero);
                        Tiles.Items.Add(tileView);
                        primero = false;
                    }
                }

                Grilla.Children.Clear();
                Grilla.Children.Add(Tiles);

                TabItemAbierto.Header = selectedItem.Title;
            }
            else
            {
                TabControlAbierto.SelectedItem = esta;
                TabControlAbierto.Items.Remove(TabItemAbierto);
            }
        }

        private TileViewItem CrearTileViewItem(string header, string content, bool maximizado = false)
        {
            // Crea instancia de UserControl
            ListaDocumentos ListadoDocs = new ListaDocumentos();

            var tileViewItem = new TileViewItem
            {
                Width = 800,
                Height = 750,
                Margin = new Thickness(5),
                Header = header,
                // Establece UserControl como contenido del TileViewItem
                Content = ListadoDocs
            };

            // Configura el estado inicial del TileViewItem
            if (maximizado)
            {
                tileViewItem.TileViewItemState = TileViewItemState.Maximized;
            }
            else
            {
                tileViewItem.TileViewItemState = TileViewItemState.Normal;
            }

            return tileViewItem;
        }


        #endregion

        #region CLICKS
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
            AbrirAgrupador();
        }

        private void AbrirAgrupador_Click(object sender, RoutedEventArgs e)
        {
            AbrirAgrupador();
        }
       
        private void EditarFicha_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                var sele = azure.Agrupadores.FirstOrDefault(a => a.ID == selectedItem.ID.ConvertToInt64Null());

                if (sele != null)
                {
                    FichaAgrupador fichaWindow = new FichaAgrupador(sele, 'P');
                    fichaWindow.AgrupadorModified += FichaWindow_AgrupadorModified;
                    fichaWindow.Show();
                }
            }
        }

        #endregion

        #region Internas

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

        #endregion
    }
}

