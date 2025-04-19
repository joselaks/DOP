using Biblioteca;
using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Documento = Biblioteca.Documento;

namespace DataObra.Interfaz.Controles
{
    public partial class UcNavegador : UserControl
    {
        #region Campos y Diccionario

        string Rol;
        List<Documento> ListaDocumentos;
        private List<byte> TiposPermitidos = new();

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    List<byte> docIDs = App.Rol == "Socio"
        //        ? TiposDeDocumento.Keys.ToList()
        //        : TiposPorRol.GetValueOrDefault(App.Rol, new());

        //    foreach (var docID in docIDs)
        //    {
        //        if (TiposDeDocumento.TryGetValue(docID, out string nombre))
        //        {
        //            var item = new MenuItem
        //            {
        //                Header = nombre,
        //                Tag = docID
        //            };
        //            item.Click += NuevoDoc_Click;
        //            NuevoMenuItem.Items.Add(item);
        //        }
        //    }
        //}


        private static readonly Dictionary<byte, string> TiposDeDocumento = new()
        {
            { 1, "Factura" }, { 2, "Plan" }, { 3, "Certificado" }, { 4, "Parte" }, { 5, "Remito" },
            { 6, "Cobro" }, { 7, "Acopio" }, { 8, "Compra" }, { 9, "Pago" }, { 10, "Presupuesto" },
            { 11, "Contrato" }, { 12, "Sueldo" }, { 13, "Pedido" }, { 14, "Ingreso" }, { 15, "Egreso" },
            { 16, "Entrada" }, { 17, "Salida" }, { 18, "Impuesto" }, { 19, "Tema" }
        };

        private static readonly Dictionary<string, List<byte>> TiposPorRol = new()
        {
            { "Presupuestos", new List<byte> { 10, 11, 4, 13 } }, // Presupuesto, Contrato, Parte, Pedido
            { "Compras", new List<byte> { 5, 6, 7, 8, 13 } }, // Remito, Cobro, Acopio, Compra, Pedido
            { "Administracion", new List<byte> { 1, 6, 9, 12, 18 } }, // Factura, Cobro, Pago, Sueldo, Impuesto
            { "JefeDeObra", new List<byte> { 2, 3, 4, 5, 13 } }, // Plan, Certificado, Parte, Remito, Pedido
            //{ "Socio", new List<byte> { 1, 2, 3, 6, 9, 10, 12, 18 } } // Acceso más amplio
        };

        #endregion

        #region Constructor

        public UcNavegador(string rol)
        {
            InitializeComponent();
            Rol = rol;
            ConfiguraRol(Rol);
            CrearMenuNuevo();
            CargarGrilla();
            CrearBotonesDinamicamente();
        }

        #endregion

        #region Métodos de Carga

        private async void CargarGrilla()
        {
            var (success, message, documentosDTO) = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(App.IdCuenta);

            if (!success)
            {
                MessageBox.Show($"Error al obtener documentos: {message}");
                return;
            }

            if (App.ListaAgrupadores?.Any() == true)
            {
                ListaDocumentos = documentosDTO
                    .Select(Convertir)
                    .Where(d => TiposPermitidos.Contains(d.TipoID))
                    .ToList();

                GrillaDocumentos.ItemsSource = ListaDocumentos;
            }
            else
            {
                //MessageBox.Show("Lista de agrupadores vacía o nula.");
                ListaDocumentos = new List<Documento>();
                GrillaDocumentos.ItemsSource = ListaDocumentos;
            }
        }


        private void CrearBotonesDinamicamente()
        {
            foreach (var tipoID in TiposPermitidos.OrderBy(id => TiposDeDocumento[id]))
            {
                var nombre = TiposDeDocumento[tipoID];
                var boton = new ToggleButton
                {
                    Tag = tipoID,
                    Content = nombre + "s",
                    Width = 85,
                    Height = 35,
                    Margin = new Thickness(3),
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Beige
                };

                boton.Checked += ToggleButton_Checked;
                boton.Unchecked += ToggleButton_Unchecked;
                items.Children.Add(boton);
            }
        }


        #endregion

        #region Conversión

        public Documento Clonar(Documento doc)
        {
            return new Documento
            {
                ID = (int)doc.ID,
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
                Active = doc.Active,
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
        }

        private static string ConvertirTipo(byte pID) =>
            TiposDeDocumento.TryGetValue(pID, out var nombre) ? nombre : "Otro";

        private static Documento Convertir(DocumentoDTO docDTO)
        {
            var agrupadoresDict = App.ListaAgrupadores?.ToDictionary(a => a.ID, a => a) ?? new();

            var doc = new Documento
            {
                ID = docDTO.ID,
                CuentaID = docDTO.CuentaID,
                TipoID = docDTO.TipoID,
                TipoDoc = ConvertirTipo(docDTO.TipoID),

                UsuarioID = docDTO.UsuarioID,
                Usuario = agrupadoresDict.GetValueOrDefault(docDTO.UsuarioID)?.Descrip,
                CreadoFecha = docDTO.CreadoFecha,

                AdminID = docDTO.AdminID,
                Admin = docDTO.AdminID.HasValue && agrupadoresDict.TryGetValue(docDTO.AdminID.Value, out var admin) ? admin.Descrip : null,

                ObraID = docDTO.ObraID,
                Obra = docDTO.ObraID.HasValue && agrupadoresDict.TryGetValue(docDTO.ObraID.Value, out var obra) ? obra.Descrip : null,

                PresupuestoID = docDTO.PresupuestoID,
                Presupuesto = docDTO.PresupuestoID.HasValue && agrupadoresDict.TryGetValue(docDTO.PresupuestoID.Value, out var presupuesto) ? presupuesto.Descrip : null,

                RubroID = docDTO.RubroID,
                Rubro = docDTO.RubroID.HasValue && agrupadoresDict.TryGetValue(docDTO.RubroID.Value, out var rubro) ? rubro.Descrip : null,

                EntidadID = docDTO.EntidadID,
                Entidad = docDTO.EntidadID.HasValue && agrupadoresDict.TryGetValue(docDTO.EntidadID.Value, out var entidad) ? entidad.Descrip : null,
                EntidadTipo = docDTO.EntidadID.HasValue && agrupadoresDict.TryGetValue(docDTO.EntidadID.Value, out var entidadTipo) ? entidadTipo.Tipo : null,

                DepositoID = docDTO.DepositoID,
                Deposito = docDTO.DepositoID.HasValue && agrupadoresDict.TryGetValue(docDTO.DepositoID.Value, out var deposito) ? deposito.Descrip : null,

                Descrip = docDTO.Descrip,
                Concepto1 = docDTO.Concepto1,
                Fecha1 = docDTO.Fecha1,
                Fecha2 = docDTO.Fecha2,
                Fecha3 = docDTO.Fecha3,
                Numero1 = docDTO.Numero1,
                Numero2 = docDTO.Numero2,
                Numero3 = docDTO.Numero3,
                Notas = docDTO.Notas,
                Active = docDTO.Active,

                Precio1 = docDTO.Precio1,
                Precio2 = docDTO.Precio2,
                Impuestos = docDTO.Impuestos,
                ImpuestosD = docDTO.ImpuestosD,

                Materiales = docDTO.Materiales,
                ManodeObra = docDTO.ManodeObra,
                Subcontratos = docDTO.Subcontratos,
                Equipos = docDTO.Equipos,
                Otros = docDTO.Otros,

                MaterialesD = docDTO.MaterialesD,
                ManodeObraD = docDTO.ManodeObraD,
                SubcontratosD = docDTO.SubcontratosD,
                EquiposD = docDTO.EquiposD,
                OtrosD = docDTO.OtrosD,

                RelDoc = docDTO.RelDoc,
                RelArt = docDTO.RelArt,
                RelMov = docDTO.RelMov,
                RelImp = docDTO.RelImp,
                RelRub = docDTO.RelRub,
                RelTar = docDTO.RelTar,
                RelIns = docDTO.RelIns,

                DetalleDocumento = new System.Collections.ObjectModel.ObservableCollection<DocumentoDetDTO>(),
                DetalleMovimientos = new List<Movimiento>(),
                DetalleImpuestos = new List<Impuesto>()
            };

            if (docDTO.EditadoID.HasValue)
            {
                doc.EditadoID = docDTO.EditadoID;
                doc.Editado = agrupadoresDict.GetValueOrDefault(docDTO.EditadoID.Value)?.Descrip;
                doc.EditadoFecha = docDTO.EditadoFecha;
            }

            if (docDTO.AutorizadoID.HasValue)
            {
                doc.AutorizadoID = docDTO.AutorizadoID;
                doc.Autorizado = agrupadoresDict.GetValueOrDefault(docDTO.AutorizadoID.Value)?.Descrip;
                doc.AutorizadoFecha = docDTO.AutorizadoFecha;
            }

            return doc;
        }


        #endregion

        #region Eventos

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.BorderBrush = Brushes.Red;
                toggleButton.BorderThickness = new Thickness(2);
                FiltrarGrilla();
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.BorderBrush = Brushes.Beige;
                toggleButton.BorderThickness = new Thickness(1);
                FiltrarGrilla();
            }
        }

        private void GrillaDocumentos_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditaDoc_Click(sender, e);

        #endregion

        #region Acciones

        private void ActualizaGrilla_Click(object sender, RoutedEventArgs e) => CargarGrilla();

        private async void BorraDoc_Click(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento sele)
            {
                var (success, message) = await DatosWeb.EliminarDocumentoAsync((int)sele.ID);
                if (success)
                {
                    if (sele.TipoID == 10)
                    {
                        // Intentar eliminar el presupuesto sin borrar los datos relacionados
                        var (presupuestoSuccess, presupuestoMessage) = await DatosWeb.EliminarPresupuestoAsync((int)sele.ID, true);
                        if (!presupuestoSuccess)
                        {
                            // Preguntar al usuario si desea volver a intentarlo borrando los datos relacionados
                            var result = MessageBox.Show(
                                $"No se pudo eliminar el presupuesto: {presupuestoMessage}\n¿Desea volver a intentarlo borrando los datos relacionados?",
                                "Error al eliminar el presupuesto",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning
                            );

                            if (result == MessageBoxResult.Yes)
                            {
                                // Intentar eliminar el presupuesto borrando los datos relacionados
                                (presupuestoSuccess, presupuestoMessage) = await DatosWeb.EliminarPresupuestoAsync((int)sele.ID, false);
                                if (!presupuestoSuccess)
                                {
                                    MessageBox.Show($"Error al eliminar el presupuesto: {presupuestoMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }

                    MessageBox.Show($"Documento eliminado con éxito. ID: {sele.ID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show($"Error al eliminar el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un documento para eliminar.");
            }
        }



        private async void EditaDoc_Click(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is not Documento sele)
            {
                MessageBox.Show("Por favor, seleccione un documento para editar.");
                return;
            }

            var copia = Clonar(sele);

            var mainWindow = Window.GetWindow(this);
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            WiDocumento ventanaDoc = sele.TipoID == 10
                ? new WiDocumento("Presupuesto", new Presupuestos.UcPresupuesto(copia))
                : new WiDocumento(" " + sele.TipoDoc, new Documentos.MaxDocumento(copia, (byte)copia.ID));

            ventanaDoc.ShowDialog();
            mainWindow.Effect = null;

            if (ventanaDoc.GuardadoConExito)
            {
                CargarGrilla(); // Método que recarga la lista o refresca el DataGrid
            }
        }

        private async void NuevoDoc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item || !int.TryParse(item.Tag?.ToString(), out int tipoDocID))
            {
                MessageBox.Show("Opción no válida");
                return;
            }

            string etiqueta = item.Header?.ToString() ?? "Nuevo";
            var mainWindow = Window.GetWindow(this);
            AplicarBlur(mainWindow, true);

            if (tipoDocID == 10) // Presupuesto
            {
                var ventanaPres = new WiDocumento(etiqueta, new Presupuestos.UcPresupuesto(null));
                ventanaPres.ShowDialog();
            }
            else
            {
                var ventanaDoc = new WiDocumento(etiqueta, new Documentos.MaxDocumento(new Documento(), (byte)tipoDocID));
                ventanaDoc.ShowDialog();
            }

            AplicarBlur(mainWindow, false);
            CargarGrilla();
        }

        private void AplicarBlur(Window ventana, bool activar)
        {
            ventana.Effect = activar ? new BlurEffect { Radius = 3 } : null;
        }

        private void FiltrarGrilla()
        {
            var tiposSeleccionados = items.Children
                .OfType<ToggleButton>()
                .Where(b => b.IsChecked == true)
                .Select(b => Convert.ToByte(b.Tag))
                .ToList();

            GrillaDocumentos.ItemsSource = tiposSeleccionados.Count == 0
                ? ListaDocumentos
                : ListaDocumentos.Where(a => tiposSeleccionados.Contains(a.TipoID)).ToList();
        }

        #endregion

        #region Roles

        private void CrearMenuNuevo()
        {
            List<byte> docIDs = Rol == "Socio"
                ? TiposDeDocumento.Keys.ToList()
                : TiposPorRol.GetValueOrDefault(Rol, new());

            foreach (var docID in docIDs)
            {
                if (TiposDeDocumento.TryGetValue(docID, out string nombre))
                {
                    var item = new MenuItem
                    {
                        Header = nombre,
                        Tag = docID
                    };
                    item.Click += NuevoDoc_Click;
                    NuevoMenuItem.Items.Add(item);
                }
            }
        }


        private void ConfiguraRol(string rol)
        {
            if (TiposPorRol.TryGetValue(rol, out var tipos))
            {
                TiposPermitidos = tipos;
            }
            else
            {
                TiposPermitidos = TiposDeDocumento.Keys.ToList(); // Si no está definido, muestra todo
            }
        }


        #endregion

        private void nuevoPres_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            WiDocumento ventanaDoc = new WiDocumento("Presupuesto", new Presupuestos.UcPresupuesto(new Documento()));
            ventanaDoc.ShowDialog();

            mainWindow.Effect = null;
            CargarGrilla();

        }
    }
}