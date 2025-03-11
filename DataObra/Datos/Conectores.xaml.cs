using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Documentos;
using Syncfusion.UI.Xaml.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Documento = Biblioteca.Documento;
using DocumentoDet = Biblioteca.DocumentoDet;

namespace DataObra.Datos
{
    public partial class Conectores : Window
    {

        public Conectores()
        {
            InitializeComponent();
            this.LogListBox.ItemsSource = DatosWeb.LogEntries; // Asigna la lista de logs a la ListBox
        }

        //Verifica un usuario, Obtiene sus datos y el Token
        private async void veriUsu_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña
            #endregion

            // Codigo a utilizar
            var respuesta = await DatosWeb.ValidarUsuarioAsync(email, pass);

            //Respuestas
            UsuarioDTO datosusuario = respuesta.Usuario.DatosUsuario;
            String Token = respuesta.Usuario.Token;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (conexionExitosa)
            {
                MessageBox.Show(datosusuario.Nombre + " " + datosusuario.Apellido);
            }
            else
            {
                MessageBox.Show("Error al conectar");

            }
        }



        // Crea un nuevo documento
        private async void nuevoDoc_Click(object sender, RoutedEventArgs e)
        {
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


        // Obtiene el detalle de un documento, especificando cuentaID,  el ID y el nombre del tipo de documento ( FacturaID, RemitoID, etc)
        private async void obtenerDocDet_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 1; // ID del documento a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            short cuentaID = 3; // ID de la cuenta
            #endregion

            // Llamar al método ObtenerDocumentosDetPorCampoAsync
            var respuesta = await DatosWeb.ObtenerDocumentosDetPorCampoAsync(id, fieldName, cuentaID);

            // Manejar la respuesta
            if (respuesta.Success)
            {
                string mensaje = $"Documentos obtenidos exitosamente. Cantidad: {respuesta.Documentos.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string mensaje = $"Error al obtener los documentos: {respuesta.Message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Edita un documento
        private async void editaDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            var documento = new Biblioteca.DTO.DocumentoDTO
            {
                ID = 19,
                CuentaID = 55,
                TipoID = 5,
                UsuarioID = 1,
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

        // Borra un documento
        private async void borraDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 19;
            #endregion

            // Llamar al método EliminarDocumentoAsync
            var (success, message) = await DatosWeb.EliminarDocumentoAsync(id);

            // Manejar la respuesta
            if (success)
            {
                MessageBox.Show($"Documento eliminado con éxito. ID: {id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al eliminar el documento: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void limpiaLogin_Click(object sender, RoutedEventArgs e)
        {
            DatosWeb.LogEntries.Clear();
        }

        // Procesar un lote de documentosDetalle
        private async void ProcesaLoteDet_Click(object sender, RoutedEventArgs e)
        {
            // Crear tres registros de DocumentoDetDTO
            var documentoDet1 = new DocumentoDetDTO
            {
                ID = 250,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 'A',
                AdminID = null,
                EntidadID = null,
                DepositoID = null,
                AcopioID = null,
                PedidoID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                RemitoID = null,
                ParteID = null,
                ObraID = null,
                PresupuestoID = null,
                RubroID = null,
                TareaID = null,
                Fecha = DateTime.Now,
                ArticuloDescrip = "Artículo 1111",
                ArticuloCantSuma = 10,
                ArticuloCantResta = 0,
                ArticuloPrecio = 100,
                SumaPesos = 1000,
                RestaPesos = 0,
                SumaDolares = 10,
                RestaDolares = 0,
                Cambio = 1,
                Accion = 'A'
            };

            var documentoDet2 = new DocumentoDetDTO
            {
                ID = 260,
                CuentaID = 2,
                UsuarioID = 2,
                Editado = DateTime.Now,
                TipoID = 'B',
                AdminID = null,
                EntidadID = null,
                DepositoID = null,
                AcopioID = null,
                PedidoID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                RemitoID = null,
                ParteID = null,
                ObraID = null,
                PresupuestoID = null,
                RubroID = null,
                TareaID = null,
                Fecha = DateTime.Now,
                ArticuloDescrip = "Artículo 2",
                ArticuloCantSuma = 20,
                ArticuloCantResta = 0,
                ArticuloPrecio = 200,
                SumaPesos = 2000,
                RestaPesos = 0,
                SumaDolares = 20,
                RestaDolares = 0,
                Cambio = 1,
                Accion = 'A'
            };

            var documentoDet3 = new DocumentoDetDTO
            {
                ID = 3000,
                CuentaID = 3,
                UsuarioID = 3,
                Editado = DateTime.Now,
                TipoID = 'C',
                AdminID = null,
                EntidadID = null,
                DepositoID = null,
                AcopioID = null,
                PedidoID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                RemitoID = null,
                ParteID = null,
                ObraID = null,
                PresupuestoID = null,
                RubroID = null,
                TareaID = null,
                Fecha = DateTime.Now,
                ArticuloDescrip = "Nuevo solo",
                ArticuloCantSuma = 30,
                ArticuloCantResta = 0,
                ArticuloPrecio = 300,
                SumaPesos = 3000,
                RestaPesos = 0,
                SumaDolares = 30,
                RestaDolares = 0,
                Cambio = 1,
                Accion = 'M'
            };

            // Crear la lista de DocumentoDetDTO
            var listaDetalleDocumento = new List<DocumentoDetDTO> { documentoDet1, documentoDet2, documentoDet3 };

            // Llamar al método ProcesarListaDetalleDocumentoAsync
            var resultado = await DatosWeb.ProcesarListaDetalleDocumentoAsync(listaDetalleDocumento);

            // Manejar el resultado de la llamada
            if (resultado.Success)
            {
                MessageBox.Show("Lista de detalles de documentos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar la lista de detalles de documentos: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void buscaCuentaDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int CuentaId = 1; // ID del impuesto a obtener
            #endregion

            var result = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(CuentaId);
            // Respuestas
            bool resultado = result.Success;
            string mensaje = result.Message;
            List<DocumentoDTO> documentos = result.Documentos;

            //Mensaje para testeo
            if (resultado == true)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documentos.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }
        }
    }
}

