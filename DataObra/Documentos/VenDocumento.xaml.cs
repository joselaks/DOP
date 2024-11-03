using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Documentos
{
    public partial class VenDocumento : Window
    {
        string Tipo;
        public Documento oActivo;

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

            if (documentosPorTipo.TryGetValue(Tipo, out var listaDocumentos))
            {
                bool primero = true;

                foreach (var item in listaDocumentos)
                {
                    // Crear instancia de Documento
                    var documento = new Documento
                    {
                        Descrip = item,
                        Notas = $"Detalles del documento {item}"
                    };

                    // Crear y agregar el TileViewItem asociado a ese documento
                    var tileView = CrearTileViewItem(item, oActivo, primero);
                    ControlDocumentos.Items.Add(tileView);
                    primero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(ControlDocumentos);
        }

        private TileViewItem CrearTileViewItem(string header, Documento documento, bool maximizado = false)
        {
            oActivo = new Documento();
            oActivo.Numero1 = 11;

            var tileViewItem = new TileViewItem
            {
                Width = 1100,
                Height = 480,
                Margin = new Thickness(5),
                Header = header,
                Content = new MaxDocumento(documento) // Muestra contenido maximizado
            };

            // Configura el estado inicial del TileViewItem
            tileViewItem.TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal;

            // Asocia el evento StateChanged
            tileViewItem.StateChanged += (s, e) => TileViewItem_StateChanged(tileViewItem, documento);

            return tileViewItem;
        }


        private bool isHandlingStateChange = false;

        private void TileViewItem_StateChanged(TileViewItem tileViewItem, Documento documento)
        {
            if (isHandlingStateChange) return;
            isHandlingStateChange = true;

            if (tileViewItem.TileViewItemState == TileViewItemState.Maximized)
            {
                tileViewItem.Content = new MaxDocumento(documento); // Muestra contenido maximizado
            }
            else
            {
                tileViewItem.Content = new MinDocumento(documento); // Muestra contenido minimizado
            }

            isHandlingStateChange = false;
        }

    }
}
