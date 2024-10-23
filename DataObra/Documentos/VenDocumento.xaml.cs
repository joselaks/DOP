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

            // Defino el TileControl
            var ControlDocumentos = new TileViewControl();
            ControlDocumentos.MinimizedItemsOrientation = MinimizedItemsOrientation.Bottom;

            if (documentosPorTipo.TryGetValue(Tipo, out var listaDocumentos))
            {
                bool primero = true;

                foreach (var item in listaDocumentos)
                {
                    var tileView = CrearTileViewItem(item, primero);
                    ControlDocumentos.Items.Add(tileView);
                    primero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(ControlDocumentos);
        }

        private TileViewItem CrearTileViewItem(string header, bool maximizado = false)
        {
            var tileViewItem = new TileViewItem
            {
                Width = 1100,
                Height = 480,
                Margin = new Thickness(5),
                Header = header,
                Content = new DataObra.Documentos.Ficha(null) 
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
