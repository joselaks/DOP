using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DataObra.Documentos
{
    public partial class VenDocumento : Window
    {
        string Tipo;

        public VenDocumento(string pTipo)
        {
            InitializeComponent();
            Tipo = pTipo;
            AbreDoc();
        }

        private void AbreDoc()
        {
            var documentosPorTipo = new Dictionary<string, string[]>
            {
                ["Facturas"] = new[] { "Compras", "Remitos", "Pagos" },
                ["Pedidos"] = new[] { "Compras", "Facturas" },
                ["Remitos"] = new[] { "Pedidos", "Compras", "Facturas" },
                ["Compras"] = new[] { "Pedidos", "Remitos", "Facturas" },
            };

            var Tiles = new TileViewControl();

            if (documentosPorTipo.TryGetValue(Tipo, out var documentos))
            {
                bool primero = true;

                foreach (var item in documentos)
                {
                    var tileView = CrearTileViewItem(item, primero);
                    Tiles.Items.Add(tileView);
                    primero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(Tiles);
        }

        private TileViewItem CrearTileViewItem(string header, bool maximizado = false)
        {
            var tileViewItem = new TileViewItem
            {
                Width = 600,
                Height = 550,
                Margin = new Thickness(5),
                Header = header,
                Content = new DataObra.Documentos.Ficha(null) // Cambia a tu UserControl
            };

            // Configura el estado inicial del TileViewItem
            tileViewItem.TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal;

            // Asocia el evento StateChanged
            tileViewItem.StateChanged += TileViewItem_StateChanged;

            return tileViewItem;
        }

        private void TileViewItem_StateChanged(object sender, EventArgs e) // Cambiado aquí
        {
            if (sender is TileViewItem tileViewItem)
            {
                if (tileViewItem.TileViewItemState == TileViewItemState.Maximized)
                {
                    tileViewItem.Content = new DataObra.Documentos.Ficha(null); // Contenido maximizado
                }
                else
                {
                    tileViewItem.Content = new ListaDocumentos(); // Contenido normal/minimizado
                }
            }
        }
    }
}
