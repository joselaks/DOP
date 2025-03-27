using Biblioteca;
using Biblioteca.DTO;
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

            var (success, message, documentos) = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(App.IdCuenta);
            // Lo que se obtiene es una lista de DocumentoDTO. Habria que convertirlos en Documentos.

            foreach (var item in documentos)
            {
                Documento doc = new Documento();
                doc = Documento.Convertir(item);

                ListaDocumentos.Add(doc);
            }

            this.GrillaDocumentos.ItemsSource = ListaDocumentos;
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

        public static Documento ConvertirDesdeDTO(DocumentoDTO dto)
        {
            return new Documento
            {
                ID = dto.ID,
                CuentaID = dto.CuentaID,  // Si es null, asigna 0
                TipoID = dto.TipoID,
                UsuarioID = dto.UsuarioID,
                CreadoFecha = dto.CreadoFecha,
                EditadoID = dto.EditadoID,
                EditadoFecha = dto.EditadoFecha,
                RevisadoID = dto.RevisadoID,
                RevisadoFecha = dto.RevisadoFecha,
                AdminID = dto.AdminID,
                ObraID = dto.ObraID,
                PresupuestoID = dto.PresupuestoID,
                RubroID = dto.RubroID,
                EntidadID = dto.EntidadID,
                DepositoID = dto.DepositoID,
                Descrip = dto.Descrip ?? string.Empty, // Evita valores null
                Concepto1 = dto.Concepto1,
                Fecha1 = dto.Fecha1,
                Fecha2 = dto.Fecha2,
                Fecha3 = dto.Fecha3,
                Numero1 = dto.Numero1,
                Numero2 = dto.Numero2,
                Numero3 = dto.Numero3,
                Notas = dto.Notas ?? string.Empty,
                Active = dto.Active,
                Pesos = dto.Pesos,
                Dolares = dto.Dolares,
                Impuestos = dto.Impuestos,
                ImpuestosD = dto.ImpuestosD,
                Materiales = dto.Materiales,
                ManodeObra = dto.ManodeObra,
                Subcontratos = dto.Subcontratos,
                Equipos = dto.Equipos,
                Otros = dto.Otros,
                MaterialesD = dto.MaterialesD,
                ManodeObraD = dto.ManodeObraD,
                SubcontratosD = dto.SubcontratosD,
                EquiposD = dto.EquiposD,
                OtrosD = dto.OtrosD,
                RelDoc = dto.RelDoc,
                RelArt = dto.RelArt,
                RelMov = dto.RelMov,
                RelImp = dto.RelImp,
                RelRub = dto.RelRub,
                RelTar = dto.RelTar,
                RelIns = dto.RelIns,
                Accion = 'M', // Asignación por defecto (Modificar)
                DetalleDocumento = new List<DocumentoDet>(),  // Inicializa listas vacías
                DetalleMovimientos = new List<Movimiento>(),
                DetalleImpuestos = new List<Impuesto>()
            };
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
            if (GrillaDocumentos.SelectedItem is DocumentoDTO sele)
            {

                var documentoSeleccionado = ConvertirDesdeDTO(sele);

                var mainWindow = Window.GetWindow(this);
                // Aplicar efecto de desenfoque a la ventana principal
                mainWindow.Effect = new BlurEffect { Radius = 3 };

                DataObra.Interfaz.Ventanas.WiDocumento ventanaDoc;

                if (documentoSeleccionado.TipoID == 10) // es un presupuesto
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











