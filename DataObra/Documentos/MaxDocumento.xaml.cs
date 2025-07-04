﻿using Biblioteca;
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
using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DataObra.Documentos
    {
    public partial class MaxDocumento : UserControl
        {
        #region Variables y Constructor
        private readonly ConcurrentQueue<string> colaMensajes = new();
        private bool mostrandoMensaje = false;

        Documento oActivo;
        string TextBoxValueAnterior;
        public bool GuardadoConExito { get; private set; } = false;
        InfoDocumento docdet = new InfoDocumento();
        public byte TipoDocID;
        public string TipoDoc;

        private static readonly Dictionary<byte, string> TiposDeDocumento = new()
        {
            { 1, "Factura" }, { 2, "Plan" }, { 3, "Certificado" }, { 4, "Parte" }, { 5, "Remito" },
            { 6, "Cobro" }, { 7, "Acopio" }, { 8, "Compra" }, { 9, "Pago" }, { 10, "Presupuesto" },
            { 11, "Contrato" }, { 12, "Sueldo" }, { 13, "Pedido" }, { 14, "Ingreso" }, { 15, "Egreso" },
            { 16, "Entrada" }, { 17, "Salida" }, { 18, "Impuesto" }, { 19, "Tema" }
        };


        public MaxDocumento(Biblioteca.Documento pDoc, byte pTipoID)
            {
            InitializeComponent();

            TipoDocID = pTipoID;
            if (TiposDeDocumento.TryGetValue(TipoDocID, out string nombreTipo))
                TipoDoc = nombreTipo + "ID"; // "FacturaID", "PedidoID", etc.
            else
                MostrarMensajeEstado($"TipoDocID {TipoDocID} no está en el diccionario.");

            // Temporal para manejar los datos en la ventana 
            docdet.DetalleDocumento = new ObservableCollection<DocumentoDetDTO>();

            this.ComboObras.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'O' && a.Active);
            this.ComboAdmin.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'A' && a.Active);
            this.ComboEntidad.ItemsSource = App.ListaAgrupadores.Where(a => new[] { 'C', 'P', 'S', 'O' }.Contains(a.TipoID) && a.Active);

            if (pDoc.ID == null)
                {
                Random random = new Random();

                oActivo = new Documento()
                    {
                    CuentaID = (byte)App.IdCuenta,
                    TipoID = TipoDocID,
                    UsuarioID = App.IdUsuario,
                    CreadoFecha = DateTime.Now,
                    EditadoID = 0,
                    EditadoFecha = DateTime.Now,
                    AutorizadoID = 0,
                    AutorizadoFecha = DateTime.Now,
                    AdminID = 63,
                    ObraID = 60,
                    PresupuestoID = 6,
                    RubroID = 6,
                    EntidadID = 64,
                    DepositoID = 5,
                    Descrip = "Descripcion " + DateTime.Today.DayOfYear,
                    Concepto1 = null,
                    Fecha1 = DateTime.Now,
                    Fecha2 = DateTime.Now.AddDays(2),
                    Fecha3 = DateTime.Now.AddDays(4),
                    Numero1 = random.Next(1, 1000),
                    Numero2 = random.Next(1001, 2000),
                    Numero3 = random.Next(2001, 3000),
                    Notas = "bb",
                    Active = false,
                    Precio1 = random.Next(5000, 19000),
                    Precio2 = random.Next(32, 999),
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

                // this.P.IsChecked = true;
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

                this.CelAutorizadoFecha.Visibility = oActivo.AutorizadoID != 0 ? Visibility.Visible : Visibility.Collapsed;
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
                AutorizadoID = doc.AutorizadoID,
                AutorizadoFecha = doc.AutorizadoFecha,
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
                Active = (bool)doc.Active,
                Precio1 = doc.Precio1,
                Precio2 = doc.Precio2,
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

        private void ComboPresupuestos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
            var sele = ComboPresupuestos.SelectedItem as Agrupador;
            if (sele != null)
                {
                oActivo.PresupuestoID = sele.ID;
                oActivo.Presupuesto = sele.Descrip;
                }
            }

        private void ComboObras_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
            var sele = ComboObras.SelectedItem as AgrupadorDTO;
            if (sele != null)
                {
                oActivo.ObraID = sele.ID;
                oActivo.Obra = sele.Descrip;
                }
            }

        private void ComboAdmin_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
            var sele = ComboAdmin.SelectedItem as AgrupadorDTO;
            if (sele != null)
                {
                oActivo.AdminID = sele.ID;
                oActivo.Admin = sele.Descrip;
                }
            }

        private void ComboEntidad_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
            bool esNuevo = false;

            if (oActivo.ID == null)
                esNuevo = true;
           
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

                MostrarMensajeEstado(mensaje);

                if (docdet.DetalleDocumento.Count > 0)
                    ActualizarDetalle();
                }
            else
            {
                MostrarMensajeEstado($"Error al {(esNuevo ? "crear" : "actualizar")} el documento: {resultado.Item2}");
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

        #region Utilidades

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
            if (oActivo.AutorizadoID == 0)
                {
                oActivo.AutorizadoID = App.IdUsuario;
                //oActivo.Autorizado = (int)App.IdUsuario;
                oActivo.AutorizadoFecha = DateTime.Today;
                }
            else
                {
                oActivo.AutorizadoID = 0;
                oActivo.Autorizado = "";
                }
            }

        private void GrillaPrincipal_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oActivo.ID != null)
            {
                ObtenerDetalle((int)oActivo.ID, TipoDoc);
            }
        }

        #endregion

        #region Documentos Detalle
        private async void ObtenerDetalle(int pDocID, string pCampo)
            {
            // Llamar al método ObtenerDocumentosDetPorCampoAsync
            var (success, message, detalles) = await DatosWeb.ObtenerDocumentosDetPorCampoAsync(pDocID, pCampo, (short)App.IdCuenta);

            if (success)
            {
                string mensaje = $"Detalles obtenidos exitosamente. Cantidad: {detalles.Count}";
                MostrarMensajeEstado(mensaje);

                foreach (var item in detalles)
                {
                    docdet.DetalleDocumento.Add(item);
                }

                // Falta Convertir de Detalles

                this.GrillaDocumentosDet.ItemsSource = docdet.DetalleDocumento;

                this.DataContext = oActivo;
                }
            else
            {
                string mensaje = $"Error al obtener los detalles: {message}";
                MostrarMensajeEstado(mensaje);
            }
        }

        private async void ActualizarDetalle()
        {
            List<DocumentoDetDTO> aguardar = new List<DocumentoDetDTO>();

            foreach (var item in docdet.DetalleDocumento)
                {
                aguardar.Add(item);
                }

            // Llamar al método ProcesarListaDetalleDocumentoAsync
            var (success, message) = await DatosWeb.ProcesarListaDetalleDocumentoAsync(aguardar);

            // Manejar el resultado de la llamada
            if (success)
            {
                MostrarMensajeEstado("Lista de detalles de documentos procesada exitosamente.");
                
                foreach (var item in docdet.DetalleDocumento.ToList())
                {
                    if (item.Accion == 'D')
                    {
                        docdet.DetalleDocumento.Remove(item);
                    }
                    else
                    {
                        item.Accion = ' ';
                    }
                }

                // GrillaDocumentosDet.ItemsSource = docdet.DetalleDocumento;
            }
            else
            {
                MostrarMensajeEstado($"Error al procesar la lista de detalles de documentos: {message}");
            }
        }

        private void OActivo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            MostrarMensajeEstado("cambio");
        }

        // Diccionario para mapear el Name del MenuItem a las columnas correspondientes
        private readonly Dictionary<string, string[]> _grupoColumnas = new Dictionary<string, string[]>
        {
            { "MenuArticulo", new[] { "ArticuloDescrip", "ArticuloCantSuma", "ArticuloCantResta", "ArticuloPrecio", "Fecha" } },
            { "MenuDocumentos", new[] { "PedidoID", "CompraID", "ContratoID", "FacturaID", "RemitoID", "ParteID" } },
            { "MenuImputaciones", new[] { "ObraID", "PresupuestoID", "RubroID", "TareaID", "InsumoID" } },
            { "MenuValores", new[] { "SumaPesos", "RestaPesos", "SumaDolares", "RestaDolares", "Cambio" } },
            { "MenuSistema", new[] { "ID", "CuentaID", "UsuarioID", "EditadoID", "TipoID" } }
        };

        // Método único columnas
        private void ToggleGrupo_Click(object sender, RoutedEventArgs e)
            {
            if (sender is not MenuItem menuItem || string.IsNullOrEmpty(menuItem.Name))
                return;

            if (_grupoColumnas.TryGetValue(menuItem.Name, out var columnas))
                {
                ToggleGrupoColumnas(columnas);
                UpdateMenuCheck(menuItem, columnas);
                }
            }

        private void ToggleGrupoColumnas(string[] columnNames)
            {
            bool? firstVisibility = null;

            foreach (var name in columnNames)
                {
                var column = GrillaDocumentosDet.Columns.FirstOrDefault(c => c.MappingName == name);
                if (column != null)
                    {
                    firstVisibility ??= column.IsHidden;
                    }
                }

            bool newVisibility = !(firstVisibility ?? false);

            foreach (var name in columnNames)
                {
                var column = GrillaDocumentosDet.Columns.FirstOrDefault(c => c.MappingName == name);
                if (column != null)
                    {
                    column.IsHidden = newVisibility;
                    }
                }
            }

        private void UpdateMenuCheck(MenuItem menuItem, string[] columnNames)
            {
            bool allHidden = columnNames.All(name =>
            {
                var col = GrillaDocumentosDet.Columns.FirstOrDefault(c => c.MappingName == name);
                return col == null || col.IsHidden;
            });

            menuItem.IsChecked = !allHidden;
            }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
            {
            if (sender is not ContextMenu menu) return;

            foreach (var item in menu.Items)
                {
                if (item is MenuItem menuItem && _grupoColumnas.TryGetValue(menuItem.Name, out var columnas))
                    {
                    bool allHidden = columnas.All(name =>
                    {
                        var col = GrillaDocumentosDet.Columns.FirstOrDefault(c => c.MappingName == name);
                        return col == null || col.IsHidden;
                    });

                    menuItem.IsChecked = !allHidden;
                    }
                }
            }

        #endregion

        private void GrillaDocumentosDet_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
            {
            var grid = sender as SfDataGrid;

            if (grid != null)
                {
                var detalle = grid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex) as DocumentoDetDTO;

                if (detalle != null)
                    {
                    if (detalle.Accion != 'A') // comillas simples para char
                        {
                        detalle.Accion = 'M';
                        }
                    }
                }
            }

        private void GrillaDocumentosDet_RecordDeleting(object sender, RecordDeletingEventArgs e)
        {
            foreach (var item in e.Items)
            {
                if (item is DocumentoDetDTO detalle)
                {
                    detalle.Accion = 'D';
                }
            }

            // Cancela el borrado
            e.Cancel = true;

            //GrillaDocumentosDet.View.Refresh();
        }


        private void NuevoDetalle_Click(object sender, RoutedEventArgs e)
        {
            var nuevoDetalle = new DocumentoDetDTO
            {
                ArticuloDescrip = "Nuevo Artículo",
                ArticuloCantSuma = 0,
                ArticuloCantResta = 0,
                ArticuloPrecio = 100,
                SumaDolares = 0,
                SumaPesos = 0,
                RestaDolares = 0,
                RestaPesos = 0,
                Fecha = DateTime.Now,
                Editado = DateTime.Now,
                TipoID = 'M',
                Accion = 'A',
                CuentaID = (byte)App.IdCuenta,
                UsuarioID = App.IdUsuario,
            };

            if (oActivo.ID != null)
                AsignarDocumento(nuevoDetalle);
            else
                MostrarMensajeEstado("Falta ID de Documento");

            docdet.DetalleDocumento.Add(nuevoDetalle);
        }

        // Agrega el ID al campo del tipo de Documento
        private void AsignarDocumento(DocumentoDetDTO pDetalle)
        {
        if (string.IsNullOrEmpty(TipoDoc)) return;

        var propiedad = typeof(DocumentoDetDTO).GetProperty(TipoDoc);

        if (propiedad != null && propiedad.CanWrite &&
            (propiedad.PropertyType == typeof(int) || propiedad.PropertyType == typeof(int?)))
            {
                propiedad.SetValue(pDetalle, oActivo.ID);
                MostrarMensajeEstado($"{TipoDoc} asignado con valor {oActivo.ID}");
            }
        else
            {
                MostrarMensajeEstado($"No se pudo asignar {TipoDoc} en pDetalle");
                // MessageBox.Show($"No se pudo asignar {TipoDoc} en pDetalle");
            }
        }

        private async void MostrarMensajeEstado(string mensaje)
        {
            colaMensajes.Enqueue(mensaje);

            if (mostrandoMensaje)
                return;

            mostrandoMensaje = true;

            while (colaMensajes.TryDequeue(out var msg))
            {
                EstadoTexto.Text = msg;
                EstadoTexto.Visibility = Visibility.Visible;

                await Task.Delay(5000); // 5 segundos

                EstadoTexto.Visibility = Visibility.Collapsed;
                EstadoTexto.Text = string.Empty;
            }

            mostrandoMensaje = false;
        }

    }
}
