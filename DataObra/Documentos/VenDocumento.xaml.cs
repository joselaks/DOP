using DataObra.Datos;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Biblioteca;
using Syncfusion.UI.Xaml.Diagram;
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
        public Biblioteca.Documento oActivo = new Biblioteca.Documento();
        DatosWeb datosWeb;
        ConsultasAPI consultasAPI;
        private readonly HttpQueueManager _queueManager;

        public VenDocumento(string pTipo, int pID)
        {
            InitializeComponent();
            consultasAPI = new ConsultasAPI();
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            this.LogListBox.ItemsSource = _queueManager.Logs;
            this.grillaLogs.ItemsSource = _queueManager.GetLogs();

            Tipo = pTipo;
            oActivo.Descrip = "RELACIONADO";

            //datosWeb = new DatosWeb();
            ObtenerDocumento(pID);
        }

        private async void ObtenerDocumento(int pID)
        {
            var docBuscado = await consultasAPI.ObtenerDocumentoPorID(pID);

            if (docBuscado.Success)
            {
                // Convierte DocumentoBiblioteca a Documento
                AbreDoc(docBuscado.doc);
            }
            else
            {
                MessageBox.Show(docBuscado.Success + " " + docBuscado.Message);
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
                Width = 1150,
                Height = 800,
                Margin = new Thickness(5),
                Header = header,
                Content = maximizado ? (object)new MaxDocumento(documento, datosWeb) : new MinDocumento(documento),
                TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal
            };

            tileViewItem.StateChanged += (s, e) => TileViewItem_StateChanged(tileViewItem, documento);

            return tileViewItem;
        }

        private void TileViewItem_StateChanged(TileViewItem tileViewItem, Biblioteca.Documento documento)
        {
            if (tileViewItem.TileViewItemState == TileViewItemState.Maximized)
            {
                tileViewItem.Content = new MaxDocumento(documento, datosWeb);
            }
            else
            {
                tileViewItem.Content = new MinDocumento(documento);
            }
        }
    }

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
}
