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
using Biblioteca.DTO;

namespace DataObra.Documentos
{
    public partial class MaxDocumento : UserControl
    {
        #region Inicializa
        Documento oActivo;
        string TextBoxValueAnterior;
        Servidor azure = new Servidor();
        #endregion

        public MaxDocumento(Biblioteca.Documento pDoc) 
        {
            InitializeComponent();
            // Resolver el objeto activo compatible con otras funciones y con los metodos del servidor
            // oActivo = pDoc;
            // si es presupuesto
            
            #region COMBOS
            this.ComboObras.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'O' && a.Active);
            this.ComboAdmin.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'A' && a.Active);
            this.ComboEntidad.ItemsSource = App.ListaAgrupadores.Where(a => new[] { 'C', 'P', 'S', 'O' }.Contains(a.TipoID) && a.Active);

            #endregion Combos

            Random random = new Random();

            if (pDoc.ID == null)
            {
                #region NUEVO
                #region Datos para testeo 

                var documento = new Biblioteca.DTO.DocumentoDTO
                {
                    CuentaID = 1,
                    TipoID = 2,
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
                //oActivo = new Documento()
                //{
                //    CuentaID = 1,
                //    TipoID = 2,
                //    UsuarioID = 3,
                //    CreadoFecha = DateTime.Now,
                //    EditadoID = 1,
                //    EditadoFecha = DateTime.Now,
                //    RevisadoID = 1,
                //    RevisadoFecha = DateTime.Now,
                //    AdminID = 63,
                //    ObraID = 60,
                //    PresupuestoID = 6,
                //    RubroID = 6,
                //    EntidadID = 64,
                //    DepositoID = 5,
                //    Descrip = "Descripcion " + DateTime.Today.DayOfYear,
                //    Concepto1 = "Concepto",
                //    Fecha1 = DateTime.Now,
                //    Fecha2 = DateTime.Now.AddDays(2),
                //    Fecha3 = DateTime.Now.AddDays(4),
                //    Numero1 = random.Next(1, 1000),
                //    Numero2 = random.Next(1001, 2000),
                //    Numero3 = random.Next(2001, 3000),
                //    Notas = "bb",
                //    Active = false,
                //    Pesos = random.Next(5000, 19000),
                //    Dolares = random.Next(32, 999),
                //    Impuestos = random.Next(1200, 4999),
                //    ImpuestosD = random.Next(19, 32),
                //    Materiales = random.Next(500, 32000),
                //    ManodeObra = random.Next(900, 1900),
                //    Subcontratos = 0,
                //    Equipos = 0,
                //    Otros = 0,
                //    MaterialesD = 0,
                //    ManodeObraD = 0,
                //    SubcontratosD = 0,
                //    EquiposD = 0,
                //    OtrosD = 0,
                //    RelDoc = false,
                //    RelArt = false,
                //    RelMov = false,
                //    RelImp = false,
                //    RelRub = false,
                //    RelTar = false,
                //    RelIns = false
                //};
                #endregion Nuevo

                this.P.IsChecked = true;
            }
            else
            {
                #region EDITA
                oActivo = pDoc;

                this.ComboObras.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.ObraID);
                this.ComboAdmin.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.AdminID);
                var entidad = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);

                if (entidad != null)
                {
                    switch (entidad.TipoID)
                    {
                        case 'C':
                            this.C.IsChecked = true;
                            break;
                        case 'P':
                            this.P.IsChecked = true;
                            break;
                        case 'S':
                            this.S.IsChecked = true;
                            break;
                        case 'O':
                            this.E.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                    this.ComboEntidad.SelectedItem = entidad;
                }

                if (oActivo.RevisadoID != 0)
                {
                    this.CelRevisadoFecha.Visibility = Visibility.Visible;
                    var revisado = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);
                    if (revisado != null) { oActivo.Revisado = revisado.Descrip; }
                }
                else
                {
                    this.CelRevisadoFecha.Visibility = Visibility.Collapsed;
                }

                if (oActivo.EditadoID != 0)
                {
                    this.CelEditadoFecha.Visibility = Visibility.Visible;
                    var editado = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);
                    if (editado != null) { oActivo.Revisado = editado.Descrip; }
                }
                else
                {
                    this.CelEditadoFecha.Visibility = Visibility.Collapsed;
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
            var sele = ComboObras.SelectedItem as AgrupadorDTO;

            if (sele != null)
            {
                oActivo.ObraID = sele.ID;
                oActivo.Obra = sele.Descrip;
            }
        }
        private void ComboAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboAdmin.SelectedItem as AgrupadorDTO;

            if (sele != null)
            {
                oActivo.AdminID = sele.ID;
                oActivo.Admin = sele.Descrip;
            }
        }
        private void ComboEntidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboEntidad.SelectedItem as AgrupadorDTO;

            if (sele != null)
            {
                oActivo.EntidadID = sele.ID;
                oActivo.Entidad = sele.Descrip;
                oActivo.EntidadTipo = sele.Tipo;
            }
        }

        #endregion

        #region PANTALLA


        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            var documento = new Biblioteca.DTO.DocumentoDTO();

            documento.CuentaID = (byte)App.IdCuenta;
            documento.TipoID = 1; // ver
            documento.UsuarioID = App.IdUsuario;
            documento.CreadoFecha = oActivo.CreadoFecha;
            documento.EditadoID = App.IdUsuario;
            documento.EditadoFecha = DateTime.Now;
            documento.AdminID = oActivo.AdminID;
            documento.ObraID = oActivo.ObraID;
            documento.PresupuestoID = oActivo.PresupuestoID;
            documento.RubroID = oActivo.RubroID;
            documento.EntidadID = oActivo.EntidadID;
            documento.DepositoID = oActivo.DepositoID;
            documento.Descrip = oActivo.Descrip;
            documento.Concepto1 = oActivo.Concepto1;
            documento.Fecha1 = oActivo.Fecha1;
            documento.Fecha2 = oActivo.Fecha2;
            documento.Fecha3 = oActivo.Fecha3;
            documento.Numero1 = oActivo.Numero1;
            documento.Numero2 = oActivo.Numero2;
            documento.Numero3 = oActivo.Numero3;
            documento.Notas = oActivo.Notas;
            documento.Active = oActivo.Active;
            documento.Pesos = oActivo.Pesos;
            documento.Dolares = oActivo.Dolares;
            documento.Impuestos = oActivo.Impuestos;
            documento.ImpuestosD = oActivo.ImpuestosD;
            documento.Materiales = oActivo.Materiales;
            documento.ManodeObra = oActivo.ManodeObra;
            documento.Subcontratos = oActivo.Subcontratos;
            documento.Equipos = oActivo.Equipos;
            documento.Otros = oActivo.Otros;
            documento.MaterialesD = oActivo.MaterialesD;
            documento.ManodeObraD = oActivo.ManodeObraD;
            documento.SubcontratosD = oActivo.SubcontratosD;
            documento.EquiposD = oActivo.EquiposD;
            documento.OtrosD = oActivo.OtrosD;
            documento.RelDoc = oActivo.RelDoc;
            documento.RelArt = oActivo.RelArt;
            documento.RelMov = oActivo.RelMov;
            documento.RelImp = oActivo.RelImp;
            documento.RelRub = oActivo.RelRub;
            documento.RelTar = oActivo.RelTar;
            documento.RelIns = oActivo.RelIns;


            // Llamar al método CrearDocumentoAsync
            var (success, message, id) = await DatosWeb.CrearDocumentoAsync(documento);

            // Manejar la respuesta
            if (success && id.HasValue)
            {
                MessageBox.Show($"Documento creado con éxito. ID: {id.Value}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al crear el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Actualizar_Click(object sender, RoutedEventArgs e)
            {
            var documento = new Biblioteca.DTO.DocumentoDTO();
            documento.ID = (int)oActivo.ID;
            documento.CuentaID = (byte)App.IdCuenta;
            documento.TipoID = 1; // ver
            documento.UsuarioID = App.IdUsuario;
            documento.CreadoFecha = oActivo.CreadoFecha;
            documento.EditadoID = App.IdUsuario;
            documento.EditadoFecha = DateTime.Now;
            documento.AdminID = oActivo.AdminID;
            documento.RevisadoID = oActivo.RevisadoID;
            documento.RevisadoFecha = oActivo.RevisadoFecha;
            documento.ObraID = oActivo.ObraID;
            documento.PresupuestoID = oActivo.PresupuestoID;
            documento.RubroID = oActivo.RubroID;
            documento.EntidadID = oActivo.EntidadID;
            documento.DepositoID = oActivo.DepositoID;
            documento.Descrip = oActivo.Descrip;
            documento.Concepto1 = oActivo.Concepto1;
            documento.Fecha1 = oActivo.Fecha1;
            documento.Fecha2 = oActivo.Fecha2;
            documento.Fecha3 = oActivo.Fecha3;
            documento.Numero1 = oActivo.Numero1;
            documento.Numero2 = oActivo.Numero2;
            documento.Numero3 = oActivo.Numero3;
            documento.Notas = oActivo.Notas;
            documento.Active = oActivo.Active;
            documento.Pesos = oActivo.Pesos;
            documento.Dolares = oActivo.Dolares;
            documento.Impuestos = oActivo.Impuestos;
            documento.ImpuestosD = oActivo.ImpuestosD;
            documento.Materiales = oActivo.Materiales;
            documento.ManodeObra = oActivo.ManodeObra;
            documento.Subcontratos = oActivo.Subcontratos;
            documento.Equipos = oActivo.Equipos;
            documento.Otros = oActivo.Otros;
            documento.MaterialesD = oActivo.MaterialesD;
            documento.ManodeObraD = oActivo.ManodeObraD;
            documento.SubcontratosD = oActivo.SubcontratosD;
            documento.EquiposD = oActivo.EquiposD;
            documento.OtrosD = oActivo.OtrosD;
            documento.RelDoc = oActivo.RelDoc;
            documento.RelArt = oActivo.RelArt;
            documento.RelMov = oActivo.RelMov;
            documento.RelImp = oActivo.RelImp;
            documento.RelRub = oActivo.RelRub;
            documento.RelTar = oActivo.RelTar;
            documento.RelIns = oActivo.RelIns;


            // Llamar al método ActualizarDocumentoAsync
            var (success, message) = await DatosWeb.ActualizarDocumentoAsync(documento);

            // Manejar la respuesta
            if (success)
                {
                MessageBox.Show($"Documento actualizado con éxito. ID: {documento.ID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al actualizar el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }




        private async void Borrar_Click(object sender, RoutedEventArgs e)
        {
            // Codigo a utilizar
            //var respuesta = await ConsultasAPI.DeleteDocumentoAsync(oActivo.ID);

            //Respuestas
            //bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            //string mensaje = respuesta.Message;

            //Mensaje para testeo
            //if (respuesta.Success != null)
            //{
            //    MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            //}
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
                char tipo = Convert.ToChar(radioButton.Name);
                this.ComboEntidad.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == tipo).OrderBy(a => a.Descrip);
                this.ComboEntidad.SelectedItem = App.ListaAgrupadores.Where(a => a.TipoID == tipo).OrderBy(a => a.Descrip).FirstOrDefault();
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
            //var docBuscado = await ConsultasAPI.ObtenerDocumentosPorCuentaID(pCuentaID);

            // Respuestas
            //bool resultado = docBuscado.Success;
            //string mensaje = docBuscado.Message;
            //List<Biblioteca.Documento> listaDocumentos = docBuscado.docs;

            //if (docBuscado.Success == true)
            //{
            //    this.GrillaDocumentos.ItemsSource = listaDocumentos;
            //}
            //else
            //{
            //    MessageBox.Show("No hay Documentos");
            //}
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

                    //var respuesta = await ConsultasAPI.PutDocumentoAsync(wDoc);
                    //MessageBox.Show(respuesta.Message, respuesta.Success ? "Éxito" : "Error");
                }
            }
        }
    }
}
