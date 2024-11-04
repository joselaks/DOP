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
        public int nro = 10;

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
                    oActivo = new Documento();
                    oActivo.Numero1 = nro;
                    nro++;

                    var tileView = CrearTileViewItem(item, oActivo, primero);
                    ControlDocumentos.Items.Add(tileView);
                    primero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(ControlDocumentos);
        }

        private TileViewItem CrearTileViewItem(string header, Documento pDocumento, bool maximizado = false)
        {
            var tileViewItem = new TileViewItem
            {
                Width = 1100,
                Height = 480,
                Margin = new Thickness(5),
                Header = header,
                Content = new MaxDocumento(pDocumento) // Muestra contenido maximizado
            };

            // Configura el estado inicial del TileViewItem
            tileViewItem.TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal;

            // Asocia el evento StateChanged
            tileViewItem.StateChanged += (s, e) => TileViewItem_StateChanged(tileViewItem, pDocumento);

            return tileViewItem;
        }

        private bool isHandlingStateChange = false;

        private void TileViewItem_StateChanged(TileViewItem tileViewItem, Documento pDocumento)
        {
            if (isHandlingStateChange) return;
            isHandlingStateChange = true;

            if (tileViewItem.TileViewItemState == TileViewItemState.Maximized)
            {
                tileViewItem.Content = new MaxDocumento(pDocumento); // Muestra contenido maximizado
            }
            else
            {
                tileViewItem.Content = new MinDocumento(pDocumento); // Muestra contenido minimizado
            }

            isHandlingStateChange = false;
        }

    }
}
