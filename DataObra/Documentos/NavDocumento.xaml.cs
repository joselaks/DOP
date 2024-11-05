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

namespace DataObra.Documentos
{
    public partial class NavDocumento : UserControl
    {
        bool angosto = true;
        Servidor azure;
        public NavDocumento(string pModo, string pDoc)
        {
            InitializeComponent();
            this.FechaDesde.SelectedDate = DateTime.Today.AddDays(-30);
            this.FechaHasta.SelectedDate = DateTime.Today;

            azure = new Servidor();

            //this.GrillaDocumentos.ItemsSource = azure.Documentos;

            _ = obtenerDocsAsync();
            //DataContext = azure.Documentos;
            
        }

        private async Task obtenerDocsAsync()
        {
            //this.GrillaDocumentos.ItemsSource = await oDatos.ObtenerDocsAsync();
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
            Button abrirButton = new Button { Name = "abrir" };
            Menu_Click(abrirButton, null);
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            int seleID = 3;

            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "nuevo":
                        VenDocumento nueva = new VenDocumento("Facturas", 0);
                        //nueva.DocumentoModified += FichaWindow_DocumentoModified;
                        nueva.Show();
                        break;

                    case "abrir":
                        Documento sele = this.GrillaDocumentos.SelectedItem as Documento;
                        
                        if (sele != null)
                        {
                            VenDocumento fichaWindow = new VenDocumento("Facturas", seleID);
                            //fichaWindow.DocumentoModified += FichaWindow_DocumentoModified;
                            fichaWindow.Show();
                        }
                        break;

                    case "borrar":
                        Documento seleBorrar = this.GrillaDocumentos.SelectedItem as Documento;
                        
                        if (seleBorrar != null)
                        {
                            var result = MessageBox.Show($"Seguro de borrar {seleBorrar.Descrip}?", "Confirmar", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                // Procedimiento para borrar del servidor
                                azure.Documentos.Remove(seleBorrar);
                                this.GrillaDocumentos.ItemsSource = null;
                                this.GrillaDocumentos.ItemsSource = azure.Documentos;
                            }
                        }
                        break;
                }
            }
        }

    }
}
