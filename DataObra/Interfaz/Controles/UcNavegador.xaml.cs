using Biblioteca;
using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Documento = Biblioteca.Documento;

namespace DataObra.Interfaz.Controles
{
    public partial class UcNavegador : UserControl
    {
        string Rol;
        List<Documento> ListaDocumentos;

        public UcNavegador(string rol)
        {
            InitializeComponent();
            Rol = rol;
            configuraRol(Rol);
            CargarGrilla();
        }

        private async void CargarGrilla()
        {
            ListaDocumentos = new List<Documento>();

            var (success, message, documentosDTO) = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(App.IdCuenta);

            if (!success)
            {
                MessageBox.Show($"Error al obtener documentos: {message}");
                return;
            }
            else
            {
                // Validar si App.ListaAgrupadores no es null y tiene elementos antes del foreach
                if (App.ListaAgrupadores?.Any() == true)
                {
                    foreach (var item in documentosDTO)
                    {
                        ListaDocumentos.Add(Convertir(item));
                    }

                    this.GrillaDocumentos.ItemsSource = ListaDocumentos;
                }
                else
                {
                    //MessageBox.Show("Lista de agrupadores vacia");
                }

            }
        }

        private static string ConvertirTipo(byte pID)
        {
            return pID switch
            {
                1 => "Factura",
                10 => "Presupuesto",
                2 => "Plan",
                3 => "Certificado",
                4 => "Parte",
                5 => "Remito",
                6 => "Cobro",
                7 => "Acopio",
                8 => "Compra",
                9 => "Pago",
                11 => "Contrato",
                12 => "Sueldo",
                13 => "Pedido",
                14 => "Ingreso",
                15 => "Egreso",
                16 => "Entrada",
                17 => "Salida",
                18 => "Impuesto",
                19 => "Tema",
                _ => "Otro"
            };
        }

        private static Documento Convertir(Biblioteca.DTO.DocumentoDTO docDTO)
        {
            var agrupadoresDict = App.ListaAgrupadores?.ToDictionary(a => a.ID, a => a) ?? new Dictionary<int, AgrupadorDTO>();

            return new Documento
            {
                #region SISTEMA
                ID = docDTO.ID,
                CuentaID = docDTO.CuentaID,
                TipoID = docDTO.TipoID,
                TipoDoc = ConvertirTipo(docDTO. TipoID),
                #endregion

                #region USUARIOS
                UsuarioID = docDTO.UsuarioID,
                Usuario = agrupadoresDict.GetValueOrDefault(docDTO.UsuarioID)?.Descrip,
                CreadoFecha = docDTO.CreadoFecha,

                EditadoID = docDTO.EditadoID,
                Editado = agrupadoresDict.GetValueOrDefault(docDTO.EditadoID)?.Descrip,
                EditadoFecha = docDTO.EditadoFecha,

                RevisadoID = docDTO.RevisadoID,
                Revisado = agrupadoresDict.GetValueOrDefault(docDTO.RevisadoID)?.Descrip,
                RevisadoFecha = docDTO.RevisadoFecha,
                #endregion

                #region AGRUPADORES
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
                #endregion

                #region DATOS
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
                #endregion

                #region TOTALES
                Pesos = docDTO.Pesos,
                Dolares = docDTO.Dolares,
                Impuestos = docDTO.Impuestos,
                ImpuestosD = docDTO.ImpuestosD,
                #endregion

                #region Por Tipo
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
                #endregion

                #region RELACIONES
                RelDoc = docDTO.RelDoc,
                RelArt = docDTO.RelArt,
                RelMov = docDTO.RelMov,
                RelImp = docDTO.RelImp,
                RelRub = docDTO.RelRub,
                RelTar = docDTO.RelTar,
                RelIns = docDTO.RelIns,
                #endregion

                #region COLECCIONES
                Accion = 'A',
                DetalleDocumento = new List<DocumentoDet>(),
                DetalleMovimientos = new List<Movimiento>(),
                DetalleImpuestos = new List<Impuesto>()
                #endregion
            };
        }

        private void configuraRol(string rol)
        {
            switch (rol)
            {
                case "Presupuestos":
                    break;
                case "Compras":
                    break;
                default:
                    break;
            }
        }

        //private async void EditaDocClick(object sender, RoutedEventArgs e)
        //{
        //    if (GrillaDocumentos.SelectedItem is DocumentoDTO documentoSeleccionado)
        //    {
                


        //        if (documentoSeleccionado.TipoID == 10) // es un presupuesto
        //        {
                    
        //            var encabezado = documentoSeleccionado;
        //            UserControl presup = new DataObra.Presupuestos.UcPresupuesto(docActivo);
        //            DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
        //            var mainWindow = Window.GetWindow(this);
        //            // Aplicar efecto de desenfoque a la ventana principal
        //            mainWindow.Effect = new BlurEffect { Radius = 3 };
        //            // Mostrar la ventana de manera modal
        //            ventanaPres.ShowDialog();
        //            // Quitar el efecto de desenfoque después de cerrar la ventana modal
        //            mainWindow.Effect = null;
        //        }
        //        else
        //        {

        //            Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(docActivo);
        //            DataObra.Interfaz.Ventanas.WiDocumento ventanaDocu = new DataObra.Interfaz.Ventanas.WiDocumento(documentoSeleccionado.TipoID.ToString(), Docu);
        //            ventanaDocu.ShowDialog();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Por favor, seleccione un documento para editar.");
        //    }
        //}

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento documentoSeleccionado)
            {
                //var respuesta = await ConsultasAPI.DeleteDocumentoAsync((int)documentoSeleccionado.ID);
                //if (respuesta.Success)
                //{
                //    MessageBox.Show("Documento eliminado exitosamente.");
                //    CargarGrilla(); // Actualizar la grilla después de eliminar el documento
                //}
                //else
                //{
                //    MessageBox.Show($"Error al eliminar el documento: {respuesta.Message}");
                //}
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un documento para eliminar.");
            }
        }

        private void NuevoPresupuesto_Click(object sender, RoutedEventArgs e)
        {
            UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
            var mainWindow = Window.GetWindow(this);

            // Aplicar efecto de desenfoque a la ventana principal
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            // Mostrar la ventana de manera modal
            ventanaPres.ShowDialog();

            // Quitar el efecto de desenfoque después de cerrar la ventana modal
            mainWindow.Effect = null;
        }

        private void NuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            Biblioteca.Documento objetoFactura = new Biblioteca.Documento();
            Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(objetoFactura);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaDocu = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", Docu);

            ventanaDocu.ShowDialog();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton != null)
            {
                toggleButton.BorderBrush = new SolidColorBrush(Colors.Red);
                toggleButton.BorderThickness = new Thickness(2);
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton != null)
            {
                toggleButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
                toggleButton.BorderThickness = new Thickness(1);
            }
        }

        private void actualizaGrilla_Click(object sender, RoutedEventArgs e)
        {
            CargarGrilla();
        }

        private void GrillaDocumentos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private async void NuevoDoc_Click(object sender, RoutedEventArgs e)
        {
                var mainWindow = Window.GetWindow(this);

                // Aplicar efecto de desenfoque a la ventana principal
                mainWindow.Effect = new BlurEffect { Radius = 3 };

                DataObra.Interfaz.Ventanas.WiDocumento ventanaDoc;

                // Agregar manejo para cada tipo de documento
                Biblioteca.Documento objetoFactura = new Biblioteca.Documento();
                Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(objetoFactura);
                ventanaDoc = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", Docu);

                // Mostrar la ventana de manera modal
                ventanaDoc.ShowDialog();

                // Quitar el efecto de desenfoque después de cerrar la ventana modal
                mainWindow.Effect = null;
        }

        private async void EditaDoc_Click(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento sele)
            {
                var mainWindow = Window.GetWindow(this);

                // Aplicar efecto de desenfoque a la ventana principal
                mainWindow.Effect = new BlurEffect { Radius = 3 };

                DataObra.Interfaz.Ventanas.WiDocumento ventanaDoc;

                if (sele.TipoID == 10) // es un presupuesto
                {
                    UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
                    ventanaDoc = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);

                }
                else
                {
                    // Agregar manejo para cada tipo de documento
                    Biblioteca.Documento objetoFactura = new Biblioteca.Documento();
                    Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(objetoFactura);
                    ventanaDoc = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", Docu);

                }
                // Mostrar la ventana de manera modal
                ventanaDoc.ShowDialog();

                // Quitar el efecto de desenfoque después de cerrar la ventana modal
                mainWindow.Effect = null;

            }
            else
            {
                MessageBox.Show("Por favor, seleccione un documento para editar.");
            }
        }

    }
}











