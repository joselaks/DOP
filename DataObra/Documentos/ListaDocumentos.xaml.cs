using DataObra.Sistema;
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

namespace DataObra.Documentos
{
    /// <summary>
    /// Lógica de interacción para ListaDocumentos.xaml
    /// </summary>
    public partial class ListaDocumentos : UserControl
    {
        Servidor azure;
        public ListaDocumentos()
        {
            InitializeComponent();

            azure = new Servidor();

            this.GrillaDocumentos.ItemsSource = azure.Documentos;
            DataContext = azure.Documentos;
        }

        private void GrillaDocs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button abrirButton = new Button { Name = "abrir" };
            Menu_Click(abrirButton, null);
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "nuevo":
                        Ficha nueva = new Ficha(null);
                        nueva.DocumentoModified += FichaWindow_DocumentoModified;
                        //nueva.Show();
                        break;
                    case "abrir":
                        Documento sele = this.GrillaDocumentos.SelectedItem as Documento;
                        if (sele != null)
                        {
                            Ficha fichaWindow = new Ficha(sele);
                            fichaWindow.DocumentoModified += FichaWindow_DocumentoModified;
                            //fichaWindow.Show();
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

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            Button abrirButton = new Button { Name = "nuevo" };
            Menu_Click(abrirButton, null);
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            Button abrirButton = new Button { Name = "abrir" };
            Menu_Click(abrirButton, null);
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            Button abrirButton = new Button { Name = "borrar" };
            Menu_Click(abrirButton, null);
        }
    }
}
