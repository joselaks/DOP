using Biblioteca;
using DataObra.Agrupadores;
using DataObra.Datos;
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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using Syncfusion.Windows.PropertyGrid;

namespace DataObra.Documentos
{
    public partial class MaxDocumento : UserControl
    {
        #region Inicializa
        Documento oActivo;
        string TextBoxValueAnterior;
        Servidor azure = new Servidor();
        Documentos.Controles.PresDetalle presDetalle;
        #endregion

        public MaxDocumento(Biblioteca.Documento pDoc) 
        {
            InitializeComponent();
            // si es presupuesto
            if (pDoc.TipoID==10)
            {
                this.grillaDetalle.Children.Clear();
                presDetalle = new Documentos.Controles.PresDetalle();
                this.grillaDetalle.Children.Add(presDetalle);

            }

            #region COMBOS
            this.ComboObras.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 1).OrderBy(a => a.Descrip);
            this.ComboAdmin.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 2).OrderBy(a => a.Descrip);
            #endregion Combos

            if (pDoc == null)
            {
                #region NUEVO
                oActivo = new Documento()
                {
                    Fecha1 = System.DateTime.Today,
                    CreadoFecha = System.DateTime.Today,
                    Usuario = azure.Usuario,
                    EditadoFecha = System.DateTime.Today,
                    EntidadID = 1
                };
                this.ComboObras.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(1);
                this.ComboAdmin.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(2);
                #endregion Nuevo
            }
            else
            {
                #region EDITA
                // Recibe el Documento a editar bajado del servidor y lo convierte
                oActivo = Documento.Convertir(pDoc);

                this.ComboObras.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.ObraID);
                this.ComboAdmin.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.AdminID);
                var entidad = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);

                if (entidad != null)
                {
                    switch (entidad.TipoID)
                    {
                        case 'C':
                            this.EsCliente.IsChecked = true;
                            break;
                        case 'P':
                            this.EsProveedor.IsChecked = true;
                            break;
                        case 'S':
                            this.EsContratista.IsChecked = true;
                            break;
                        case 'O':
                            this.EsPersonal.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                    this.ComboEntidad.SelectedItem = entidad;
                }

                if (oActivo.RevisadoID != 0)
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Visible;
                }
                else
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Collapsed;
                }
                #endregion Nuevo
            }

            this.DataContext = oActivo;
        }

        private void OActivo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            MessageBox.Show("cambio");
        }

        #region COMBOS

        private void ComboPresupuestos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboPresupuestos.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.PresupuestoID = sele.ID;
                oActivo.Presupuesto = sele.Descrip;
            }
        }

        private void ComboObras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboObras.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.ObraID = sele.ID;
                oActivo.Obra = sele.Descrip;
            }
        }
        private void ComboAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboAdmin.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.AdminID = sele.ID;
                oActivo.Admin = sele.Descrip;
            }
        }
        private void ComboEntidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboEntidad.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.EntidadID = sele.ID;
                oActivo.Entidad = sele.Descrip;
                oActivo.EntidadTipo = EntidadTipoHelper.GetEntidadTipo(sele.ID);
            }
        }

        #endregion

        #region PANTALLA

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            oActivo.EditadoID = azure.UsuarioID;
            oActivo.Editado = azure.Usuario;
            oActivo.EditadoFecha = DateTime.Now;

            Biblioteca.Documento wDoc = new Biblioteca.Documento();
            wDoc = Documento.ConvertirInverso(oActivo);

            //Si el documento es nuevo.....

            #region Datos para testeo 

            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
                CuentaID = 1,
                TipoID = 10,
                UsuarioID = 3,
                CreadoFecha = DateTime.Now,
                EditadoID = 4,
                EditadoFecha = DateTime.Now,
                RevisadoID = 5,
                RevisadoFecha = DateTime.Now,
                AdminID = 3,
                ObraID = 5,
                PresupuestoID = 6,
                RubroID = 6,
                EntidadID = 7,
                DepositoID = 5,
                Descrip = "a",
                Concepto1 = "b",
                Fecha1 = DateTime.Now,
                Fecha2 = DateTime.Now,

                Fecha3 = DateTime.Now,
                Numero1 = 0,
                Numero2 = 0,
                Numero3 = 0,
                Notas = "bb",
                Active = false,
                Pesos = 0,
                Dolares = 0,
                Impuestos = 0,
                ImpuestosD = 0,
                Materiales = 0,
                ManodeObra = 0,
                Subcontratos = 0,
                Equipos = 0,
                Otros = 0,
                MaterialesD = 0,
                ManodeObraD = 0,
                SubcontratosD = 0,
                EquiposD = 0,
                OtrosD = 0,
                RelDoc = false,
                RelArt = false,
                RelMov = false,
                RelImp = false,
                RelRub = false,
                RelTar = false,
                RelIns = false
            };

            #endregion

            var respuesta = await ConsultasAPI.PostDocumentoAsync(documento);
            //Respuestas
            int? nuevodoc = respuesta.Id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());

            //Si es existente

            //var (success, message) = await ConsultasAPI.PutDocumentoAsync(wDoc);

            //Window ventanaPadre = Window.GetWindow(this);
            //if (ventanaPadre != null)
            //{
            //    MessageBox.Show(message, success ? "Éxito" : "Error");
            //    ventanaPadre.Close();
            //}
        }

        private async void Borrar_Click(object sender, RoutedEventArgs e)
        {
            // Codigo a utilizar
            var respuesta = await ConsultasAPI.DeleteDocumentoAsync(oActivo.ID);

            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }
        }
        
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener la ventana que contiene este UserControl
            Window ventanaPadre = Window.GetWindow(this);

            if (ventanaPadre != null)
            {
                //if (Modificado)
                //{
                //    var result = MessageBox.Show("Hay cambios sin guardar. ¿Está seguro de que desea cerrar?", "Confirmar cierre", MessageBoxButton.YesNo);
                //    if (result == MessageBoxResult.No)
                //    {
                //        return; // No cerrar la ventana
                //    }
                //}
                ventanaPadre.Close();
            }
        }


        private void EntityType_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                var tipoDiccionario = new Dictionary<string, int>
                {
                    { "EsCliente", 10 },
                    { "EsProveedor", 20 },
                    { "EsContratista", 30 },
                    { "EsPersonal", 40 }
                };

                if (tipoDiccionario.TryGetValue(radioButton.Name, out int tipoID))
                {
                    this.ComboEntidad.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == tipoID).OrderBy(a => a.Descrip);
                    this.ComboEntidad.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(tipoID);
                }
            }
        }

        private void BotonVerifica_Click(object sender, RoutedEventArgs e)
        {
            if (oActivo.RevisadoID == 0)
            {
                oActivo.RevisadoID = azure.UsuarioID;
                oActivo.Revisado = azure.Usuario;
                oActivo.RevisadoFecha = System.DateTime.Today;
            }
            else
            {
                oActivo.RevisadoID = 0;
                oActivo.Revisado = "";
            }
        }

        #endregion Pantalla

        private async void ListaDocumentosAsync(short pCuentaID)
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
            }
            else
            {
                MessageBox.Show("No hay Documentos");
            }
        }

        private void GrillaPrincipal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListaDocumentosAsync(1);
        }

        private void Vincular_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el ítem seleccionado desde el SfDataGrid
            var selectedItem = GrillaDocumentos.SelectedItem as Biblioteca.Documento;

            if (selectedItem != null)
            {
                // Implementa tu lógica aquí
                MessageBox.Show($"Seleccionado: ID {selectedItem.ID}, Descripción: {selectedItem.Descrip}");
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún ítem.");
            }
        }

        private void DesVincular_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                TextBoxValueAnterior = textBox.Text;
            }
        }

     

        private async void cFecha_DateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (oActivo != null && e.NewValue != e.OldValue)
                {
                    switch (element.Name)
                    {
                        case "cFecha1":
                            oActivo.Fecha1 = (DateTime)e.NewValue;
                            break;
                        case "cFecha2":
                            oActivo.Fecha2 = (DateTime?)e.NewValue;
                            break;
                        case "cFecha3":
                            oActivo.Fecha3 = (DateTime?)e.NewValue;
                            break;
                        default:
                            break;
                    }

                    Biblioteca.Documento wDoc = new Biblioteca.Documento();
                    wDoc = Documento.ConvertirInverso(oActivo);

                    var respuesta = await ConsultasAPI.PutDocumentoAsync(wDoc);
                    MessageBox.Show(respuesta.Message, respuesta.Success ? "Éxito" : "Error");
                }
            }
        }
    }
}
