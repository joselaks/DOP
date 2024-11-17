using DataObra.Insumos.Clases;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataObra.Sistema;
using DataObra.Datos;

namespace DataObra.Documentos
{
    public partial class NavDocumento : UserControl
    {
        #region Inicializa
        ConsultasAPI ConsultasAPI;
        
        bool angosto = true;
        Servidor azure;
        #endregion

        public NavDocumento(string pModo, string pDoc)
        {
            InitializeComponent();
            ConsultasAPI = new ConsultasAPI();
            this.FechaDesde.SelectedDate = DateTime.Today.AddDays(-30);
            this.FechaHasta.SelectedDate = DateTime.Today;
            azure = new Servidor();

            ListaDocumentosAsync(1);
        }

        private async void ListaDocumentosAsync(byte pCuentaID)
        {
            // Código a utilizar
            var docBuscado = await ConsultasAPI.ObtenerDocumentosPorCuentaID(pCuentaID);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<Biblioteca.Documento> listaDocumentos = docBuscado.docs;

            if (docBuscado.Success == true)
            {
                this.GrillaDocumentos.ItemsSource = listaDocumentos;

                //MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + listaDocumentos.Count());
                this.BarraEstado.Content = "Se obtuvieron " + listaDocumentos.Count() + " documentos.";
            }
            else
            {
                MessageBox.Show("No hay Documentos");
            }
        }

        private void FichaWindow_DocumentoModified(object sender, Documento e)
        {
            UpdateDocumentoInCollection(e);
            this.GrillaDocumentos.ItemsSource = null;
            this.GrillaDocumentos.ItemsSource = azure.Documentos;
        }

        private void UpdateDocumentoInCollection(Documento updatedDocumento)
        {
            var existingDocumento = azure.Documentos.FirstOrDefault(d => d.ID == updatedDocumento.ID);
            if (existingDocumento != null)
            {
                int index = azure.Documentos.IndexOf(existingDocumento);
                azure.Documentos[index] = updatedDocumento;
            }
            else
            {
                azure.Documentos.Add(updatedDocumento);
            }

            // Asegurarse de que la selección se actualiza en la grilla
            Dispatcher.Invoke(() =>
            {
                GrillaDocumentos.SelectedItem = updatedDocumento;
                //GrillaDocumentos.ScrollIntoView(updatedDocumento);
            });
        }

        private void Lateral_Click(object sender, RoutedEventArgs e)
        {
            if (angosto)
            {
                Grilla.ColumnDefinitions[0].Width = new GridLength(490);
                angosto = false;
            }
            else
            {
                Grilla.ColumnDefinitions[0].Width = new GridLength(230);
                angosto = true;
            }
        }

        private void MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button abrirButton = new Button { Name = "Editar" };
            Menu_Click(abrirButton, null);
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "Nuevo":
                    case "NuevoDoc":
                        VenDocumento nueva = new VenDocumento("Facturas", 0, null);
                        //nueva.DocumentoModified += FichaWindow_DocumentoModified;
                        nueva.Show();
                        break;

                    case "Editar":
                    case "EditarDoc":
                        Biblioteca.Documento sele = this.GrillaDocumentos.SelectedItem as Biblioteca.Documento;
                        
                        if (sele != null)
                        {
                            VenDocumento fichaWindow = new VenDocumento("Facturas", sele.ID, ConsultasAPI);
                            //fichaWindow.DocumentoModified += FichaWindow_DocumentoModified;
                            fichaWindow.Show();
                        }
                        break;

                    case "Borrar":
                    case "BorrarDoc":
                        Biblioteca.Documento seleBorrar = this.GrillaDocumentos.SelectedItem as Biblioteca.Documento;

                        if (seleBorrar != null)
                        {
                            var result = MessageBox.Show($"Seguro de borrar {seleBorrar.Descrip}?", "Confirmar", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                // Procedimiento para borrar del servidor
                                BorrarDocumento(seleBorrar.ID);
                                this.GrillaDocumentos.ItemsSource = null;
                                ListaDocumentosAsync(1);
                            }
                        }
                        break;
                }
            }
        }

        private async void BorrarDocumento(int pDocID)
        {
            // Codigo a utilizar
            var respuesta = await ConsultasAPI.DeleteDocumentoAsync(pDocID);

            // Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            // Mensaje para testeo
            if (respuesta.Success != null)
            {
                //MessageBox.Show(respuesta.Success + " " + respuesta.Message);
                this.BarraEstado.Content = "Se elimino el documento con ID " + pDocID.ToString();
            }

        }
    }
}
