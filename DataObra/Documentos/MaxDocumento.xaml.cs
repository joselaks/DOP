using Biblioteca;
using DataObra.Agrupadores;
using DataObra.Datos;
using DataObra.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Biblioteca.DTO;

namespace DataObra.Documentos
{
    public partial class MaxDocumento : UserControl
    {
        #region Variables y Constructor

        Documento oActivo;
        string TextBoxValueAnterior;
        public bool GuardadoConExito { get; private set; } = false;

        public MaxDocumento(Biblioteca.Documento pDoc, byte TipoID)
        {
            InitializeComponent();

            this.ComboObras.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'O' && a.Active);
            this.ComboAdmin.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'A' && a.Active);
            this.ComboEntidad.ItemsSource = App.ListaAgrupadores.Where(a => new[] { 'C', 'P', 'S', 'O' }.Contains(a.TipoID) && a.Active);

            Random random = new Random();

            if (pDoc.ID == null)
            {
                oActivo = new Documento()
                {
                    CuentaID = 1,
                    TipoID = 2,
                    UsuarioID = 3,
                    CreadoFecha = DateTime.Now,
                    EditadoID = 1,
                    EditadoFecha = DateTime.Now,
                    RevisadoID = 1,
                    RevisadoFecha = DateTime.Now,
                    AdminID = 63,
                    ObraID = 60,
                    PresupuestoID = 6,
                    RubroID = 6,
                    EntidadID = 64,
                    DepositoID = 5,
                    Descrip = "Descripcion " + DateTime.Today.DayOfYear,
                    Concepto1 = "Concepto",
                    Fecha1 = DateTime.Now,
                    Fecha2 = DateTime.Now.AddDays(2),
                    Fecha3 = DateTime.Now.AddDays(4),
                    Numero1 = random.Next(1, 1000),
                    Numero2 = random.Next(1001, 2000),
                    Numero3 = random.Next(2001, 3000),
                    Notas = "bb",
                    Active = false,
                    Pesos = random.Next(5000, 19000),
                    Dolares = random.Next(32, 999),
                    Impuestos = random.Next(1200, 4999),
                    ImpuestosD = random.Next(19, 32),
                    Materiales = random.Next(500, 32000),
                    ManodeObra = random.Next(900, 1900),
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
                this.P.IsChecked = true;
            }
            else
            {
                oActivo = pDoc;

                this.ComboObras.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.ObraID);
                this.ComboAdmin.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.AdminID);
                var entidad = App.ListaAgrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);

                if (entidad != null)
                {
                    switch (entidad.TipoID)
                    {
                        case 'C': this.C.IsChecked = true; break;
                        case 'P': this.P.IsChecked = true; break;
                        case 'S': this.S.IsChecked = true; break;
                        case 'O': this.E.IsChecked = true; break;
                    }
                    this.ComboEntidad.SelectedItem = entidad;
                }

                this.CelRevisadoFecha.Visibility = oActivo.RevisadoID != 0 ? Visibility.Visible : Visibility.Collapsed;
                this.CelEditadoFecha.Visibility = oActivo.EditadoID != 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            this.DataContext = oActivo;
        }

        #endregion

        #region Métodos de Conversión

    



        private static DocumentoDTO ConvertirADTO(Documento doc, bool esNuevo)
        {
            var dto = new DocumentoDTO
            {
                CuentaID = doc.CuentaID,
                TipoID = doc.TipoID,
                UsuarioID = doc.UsuarioID,
                CreadoFecha = doc.CreadoFecha,
                EditadoID = doc.EditadoID,
                EditadoFecha = doc.EditadoFecha,
                RevisadoID = doc.RevisadoID,
                RevisadoFecha = doc.RevisadoFecha,
                AdminID = doc.AdminID,
                ObraID = doc.ObraID,
                PresupuestoID = doc.PresupuestoID,
                RubroID = doc.RubroID,
                EntidadID = doc.EntidadID,
                DepositoID = doc.DepositoID,
                Descrip = doc.Descrip,
                Concepto1 = doc.Concepto1,
                Fecha1 = doc.Fecha1,
                Fecha2 = doc.Fecha2,
                Fecha3 = doc.Fecha3,
                Numero1 = doc.Numero1,
                Numero2 = doc.Numero2,
                Numero3 = doc.Numero3,
                Notas = doc.Notas,
                Active = doc.Active,
                Pesos = doc.Pesos,
                Dolares = doc.Dolares,
                Impuestos = doc.Impuestos,
                ImpuestosD = doc.ImpuestosD,
                Materiales = doc.Materiales,
                ManodeObra = doc.ManodeObra,
                Subcontratos = doc.Subcontratos,
                Equipos = doc.Equipos,
                Otros = doc.Otros,
                MaterialesD = doc.MaterialesD,
                ManodeObraD = doc.ManodeObraD,
                SubcontratosD = doc.SubcontratosD,
                EquiposD = doc.EquiposD,
                OtrosD = doc.OtrosD,
                RelDoc = doc.RelDoc,
                RelArt = doc.RelArt,
                RelMov = doc.RelMov,
                RelImp = doc.RelImp,
                RelRub = doc.RelRub,
                RelTar = doc.RelTar,
                RelIns = doc.RelIns
            };

            if (!esNuevo)
                dto.ID = (int)doc.ID;

            return dto;

        }

        #endregion

        #region Eventos de ComboBox

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

        #region Eventos de Fecha

        private async void cFecha_DateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && oActivo != null && e.NewValue != e.OldValue)
            {
                switch (element.Name)
                {
                    case "cFecha1": oActivo.Fecha1 = (DateTime)e.NewValue; break;
                    case "cFecha2": oActivo.Fecha2 = (DateTime?)e.NewValue; break;
                    case "cFecha3": oActivo.Fecha3 = (DateTime?)e.NewValue; break;
                }
            }
        }

        #endregion

        #region Acciones de Botones

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            bool esNuevo = oActivo.ID == null;

            var documento = ConvertirADTO(oActivo, esNuevo);

            (bool, string, int?) EmpaquetarResultado((bool, string) r, int? id) => (r.Item1, r.Item2, id);

            var resultado = esNuevo
                ? await DatosWeb.CrearDocumentoAsync(documento)
                : EmpaquetarResultado(await DatosWeb.ActualizarDocumentoAsync(documento), documento.ID);

            if (resultado.Item1 && resultado.Item3.HasValue)
            {
                GuardadoConExito = true;
                string mensaje = esNuevo
                    ? $"Documento creado con éxito. ID asignado: {resultado.Item3.Value}"
                    : $"Documento actualizado con éxito. ID: {resultado.Item3.Value}";

                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Error al {(esNuevo ? "crear" : "actualizar")} el documento: {resultado.Item2}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }




        private async void Borrar_Click(object sender, RoutedEventArgs e)
        {
            if (oActivo.ID != null)
            {
                var (success, message) = await DatosWeb.EliminarDocumentoAsync((int)oActivo.ID);

                if (success)
                {
                    MessageBox.Show($"Documento eliminado con éxito. ID: {oActivo.ID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    MessageBox.Show($"Error al eliminar el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        #endregion

        #region Lógica de Vinculación

        private void Vincular_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GrillaDocumentos.SelectedItem as Biblioteca.Documento;
            if (selectedItem != null)
                MessageBox.Show($"Seleccionado: ID {selectedItem.ID}, Descripción: {selectedItem.Descrip}");
            else
                MessageBox.Show("No se ha seleccionado ningún ítem.");
        }

        private void DesVincular_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Utilidades y Otros Eventos

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
                TextBoxValueAnterior = textBox.Text;
        }

        private void EntityType_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                char tipo = Convert.ToChar(radioButton.Name);
                this.ComboEntidad.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == tipo).OrderBy(a => a.Descrip);
                this.ComboEntidad.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.TipoID == tipo);
            }
        }

        private void BotonVerifica_Click(object sender, RoutedEventArgs e)
        {
            if (oActivo.RevisadoID == 0)
            {
                oActivo.RevisadoID = App.IdUsuario;
                //oActivo.Revisado = (int)App.IdUsuario;
                oActivo.RevisadoFecha = DateTime.Today;
            }
            else
            {
                oActivo.RevisadoID = 0;
                oActivo.Revisado = "";
            }
        }

        private void GrillaPrincipal_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void OActivo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            MessageBox.Show("cambio");
        }

        #endregion
    }
}
