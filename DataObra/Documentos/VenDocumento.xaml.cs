using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
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

namespace DataObra.Documentos
{
    public partial class VenDocumento : Window
    {
        string Tipo;
        public VenDocumento(string pTipo)
        {
            InitializeComponent();
            Tipo = pTipo;
        }

        private void AbreDoc()
        {
            // Mapeo de tipos a colecciones de documentos
            var documentosPorTipo = new Dictionary<string, string[]>
            {
                ["Facturas"] = new[] { "Compras", "Remitos", "Pagos" },
                ["Pedidos"] = new[] { "Compras", "Facturas" },
                ["Remitos"] = new[] { "Pedidos", "Compras", "Facturas" },
                ["Compras"] = new[] { "Pedidos", "Remitos", "Facturas" },
            };

            TileViewControl Tiles = new TileViewControl();

            if (documentosPorTipo.TryGetValue(Tipo, out var documentos))
            {
                bool primero = true;

                foreach (var item in documentos)
                {
                    var tileView = CrearTileViewItem(item, "xx", primero);
                    Tiles.Items.Add(tileView);
                    primero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(Tiles);

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

    }
}
