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
using Syncfusion.PMML;

namespace DataObra.Documentos
{
    public partial class VenDocumento : Window
    {
        string Tipo;
        public Biblioteca.Documento oActivo = new Biblioteca.Documento();
        DatosWeb datosWeb;
        ConsultasAPI ConsultasAPI;

        public VenDocumento(string pTipo, int pID, ConsultasAPI pConsultasAPI)
        {
            InitializeComponent();
            Tipo = pTipo;
            ConsultasAPI = pConsultasAPI;

            if (pID != 0)
            {
                ObtenerDocumento(pID);
            }
            else
            {
                AbreDoc(null);
            }
        }

        private async void ObtenerDocumento(int pID)
        {
            var docBuscado = await ConsultasAPI.ObtenerDocumentoPorID(pID);

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
            if (pDocumento != null)
            {
                BuscaDocRel(pDocumento.ID);

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
            else
            {
                var nuevoTile = CrearTileViewItem("Nuevo", true ? pDocumento : oActivo, true);
                ControlDocumentos.Items.Add(nuevoTile);
                GrillaVenDocumento.Children.Clear();
                GrillaVenDocumento.Children.Add(ControlDocumentos);
            }
        }

        // Obtiene las relaciones de un documento desde el superiorID
        private async void BuscaDocRel(int pSuperiorID)
        {
            // Código a utilizar
            var docBuscado = await ConsultasAPI.GetDocumentosRelPorSupIDAsync(pSuperiorID);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<Biblioteca.DocumentoRel> documento = docBuscado.DocumentosRel;

            //Mensaje para testeo
            if (docBuscado.Success == true)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documento.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }
        }

        private TileViewItem CrearTileViewItem(string header, Biblioteca.Documento documento, bool maximizado = false)
        {
            var tileViewItem = new TileViewItem
            {
                Width = 1150,
                Height = 800,
                Margin = new Thickness(5),
                Header = header,
                Content = maximizado ? (object)new MaxDocumento(documento) : new MinDocumento(documento),
                TileViewItemState = maximizado ? TileViewItemState.Maximized : TileViewItemState.Normal
            };

            tileViewItem.StateChanged += (s, e) => TileViewItem_StateChanged(tileViewItem, documento);

            return tileViewItem;
        }

        private void TileViewItem_StateChanged(TileViewItem tileViewItem, Biblioteca.Documento documento)
        {
            if (tileViewItem.TileViewItemState == TileViewItemState.Maximized)
            {
                tileViewItem.Content = new MaxDocumento(documento);
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
