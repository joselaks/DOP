using DataObra.Datos;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Documentos
{
    public static class DocumentosTipos
    {
        public static readonly Dictionary<string, string[]> DocumentosPorTipo = new Dictionary<string, string[]>
        {
            ["Facturas"] = new[] { "Compra", "Remito 232", "Pago 52", "Pago 22", "Pago 44" },
            ["Pedidos"] = new[] { "Compra", "Factura" },
            ["Remitos"] = new[] { "Pedido", "Compra", "Factura" },
            ["Compras"] = new[] { "Pedido", "Remito", "Factura" },
        };
    }

    public partial class VenDocumento : Window
    {
        string Tipo;
        public Biblioteca.Documento oActivo = new Biblioteca.Documento();
        DatosWeb datosWeb;

        public VenDocumento(string pTipo, int pID)
        {
            InitializeComponent();
            Tipo = pTipo;
            oActivo.Descrip = "RELACIONADO";

            datosWeb = new DatosWeb();
            ObtenerDocumento(pID);
        }

        private async void ObtenerDocumento(int pID)
        {
            var (success, message, documento) = await datosWeb.ObtenerDocumentoPorIDAsync(pID);

            if (success)
            {
                // Convierte DocumentoBibloteca a Documento
                AbreDoc(documento);
            }
            else
            {
                MessageBox.Show("No encontrado en servidor");
            }
        }

        private void AbreDoc(Biblioteca.Documento pDocumento)
        {
            if (DocumentosTipos.DocumentosPorTipo.TryGetValue(Tipo, out var listaDocumentos))
            {
                bool esPrimero = true;

                foreach (var item in listaDocumentos) // Cambiar por la colección de docs relacionados
                {
                    var documento = new Biblioteca.Documento
                    {
                        Descrip = item,
                        Notas = $"Detalles del documento {item}"
                    };

                    var nuevoTile = CrearTileViewItem(item, esPrimero ? pDocumento : oActivo, esPrimero);

                    ControlDocumentos.Items.Add(nuevoTile);
                    esPrimero = false;
                }
            }

            GrillaVenDocumento.Children.Clear();
            GrillaVenDocumento.Children.Add(ControlDocumentos);
        }

        private TileViewItem CrearTileViewItem(string header, Biblioteca.Documento documento, bool maximizado = false)
        {
            var tileViewItem = new TileViewItem
            {
                Width = 1100,
                Height = 480,
                Margin = new Thickness(5),
                Header = header,
                Content = maximizado ? (object)new MaxDocumento(documento,datosWeb) : new MinDocumento(documento),
                TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal
            };

            tileViewItem.StateChanged += (s, e) => TileViewItem_StateChanged(tileViewItem, documento);

            return tileViewItem;
        }

        private void TileViewItem_StateChanged(TileViewItem tileViewItem, Biblioteca.Documento documento) 
        { 
            if (tileViewItem.TileViewItemState == TileViewItemState.Maximized) 
            {
                tileViewItem.Content = new MaxDocumento(documento,datosWeb); 
            }
            else
            {
                tileViewItem.Content = new MinDocumento(documento); 
            }
        }
    }
}
