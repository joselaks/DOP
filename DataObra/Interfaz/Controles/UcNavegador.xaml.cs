using Biblioteca;
using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using DataObra.Interfaz.Ventanas;
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

        private static readonly Dictionary<byte, string> TiposDeDocumento = new()
        {
            { 1, "Factura" }, { 2, "Plan" }, { 3, "Certificado" }, { 4, "Parte" }, { 5, "Remito" },
            { 6, "Cobro" }, { 7, "Acopio" }, { 8, "Compra" }, { 9, "Pago" }, { 10, "Presupuesto" },
            { 11, "Contrato" }, { 12, "Sueldo" }, { 13, "Pedido" }, { 14, "Ingreso" }, { 15, "Egreso" },
            { 16, "Entrada" }, { 17, "Salida" }, { 18, "Impuesto" }, { 19, "Tema" }
        };

        #endregion

        #region Constructor

        public UcNavegador(string rol)
        {
            InitializeComponent();
            Rol = rol;
            ConfiguraRol(Rol);
            CargarGrilla();
            CrearBotonesDinamicamente();
        }

        #endregion

        #region Métodos de Carga

        private async void CargarGrilla()
        {
            ListaDocumentos = new();
            var (success, message, documentosDTO) = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(App.IdCuenta);

            if (!success)
            {
                MessageBox.Show($"Error al obtener documentos: {message}");
                return;
            }

            if (App.ListaAgrupadores?.Any() == true)
            {
                foreach (var item in documentosDTO)
                    ListaDocumentos.Add(Convertir(item));

                GrillaDocumentos.ItemsSource = ListaDocumentos;
            }
        }

        private void CrearBotonesDinamicamente()
        {
            foreach (var kvp in TiposDeDocumento.OrderBy(k => k.Value))
            {
                var boton = new ToggleButton
                {
                    Tag = kvp.Key,
                    Content = kvp.Value + "s",
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

        private static string ConvertirTipo(byte pID) =>
            TiposDeDocumento.TryGetValue(pID, out var nombre) ? nombre : "Otro";

        private static Documento Convertir(DocumentoDTO docDTO)
        {
            var agrupadoresDict = App.ListaAgrupadores?.ToDictionary(a => a.ID, a => a) ?? new();

            return new Documento
            {
                ID = docDTO.ID,
                CuentaID = docDTO.CuentaID,
                TipoID = docDTO.TipoID,
                TipoDoc = ConvertirTipo(docDTO.TipoID),

                UsuarioID = docDTO.UsuarioID,
                Usuario = agrupadoresDict.GetValueOrDefault(docDTO.UsuarioID)?.Descrip,
                CreadoFecha = docDTO.CreadoFecha,

                EditadoID = docDTO.EditadoID,
                Editado = agrupadoresDict.GetValueOrDefault(docDTO.EditadoID)?.Descrip,
                EditadoFecha = docDTO.EditadoFecha,

                RevisadoID = docDTO.RevisadoID,
                Revisado = agrupadoresDict.GetValueOrDefault(docDTO.RevisadoID)?.Descrip,
                RevisadoFecha = docDTO.RevisadoFecha,

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

                Pesos = docDTO.Pesos,
                Dolares = docDTO.Dolares,
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

                Accion = 'A',
                DetalleDocumento = new List<DocumentoDet>(),
                DetalleMovimientos = new List<Movimiento>(),
                DetalleImpuestos = new List<Impuesto>()
            };
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
                    MessageBox.Show($"Documento eliminado con éxito. ID: {sele.ID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show($"Error al eliminar el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Seleccione un documento para eliminar.");
        }

        private async void EditaDoc_Click(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is not Documento sele)
            {
                MessageBox.Show("Por favor, seleccione un documento para editar.");
                return;
            }

            var mainWindow = Window.GetWindow(this);
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            WiDocumento ventanaDoc = sele.TipoID == 10
                ? new WiDocumento("Presupuesto", new Presupuestos.UcPresupuesto(null))
                : new WiDocumento(" " + sele.TipoDoc, new Documentos.MaxDocumento(sele));

            ventanaDoc.ShowDialog();
            mainWindow.Effect = null;
        }

        private async void NuevoDoc_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            WiDocumento ventanaDoc = new("Factura", new Documentos.MaxDocumento(new Documento()));
            ventanaDoc.ShowDialog();

            mainWindow.Effect = null;
            CargarGrilla();
        }

        private void NuevoPresupuesto_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            var ventanaPres = new WiDocumento("Presupuesto", new Presupuestos.UcPresupuesto(null));
            ventanaPres.ShowDialog();

            mainWindow.Effect = null;
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

        private void ConfiguraRol(string rol)
        {
            switch (rol)
            {
                case "Presupuestos":
                case "Compras":
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}