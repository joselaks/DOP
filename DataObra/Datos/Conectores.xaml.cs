using Biblioteca;
using DataObra.Agrupadores;
using Syncfusion.UI.Xaml.Diagram;
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
using System.Windows.Shapes;

namespace DataObra.Datos
{
    public partial class Conectores : Window
    {
        ConsultasAPI consultasAPI;
        private readonly HttpQueueManager _queueManager;

        public Conectores()
        {
            InitializeComponent();
            consultasAPI = new ConsultasAPI();
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            this.LogListBox.ItemsSource = _queueManager.Logs;
            this.grillaLogs.ItemsSource = _queueManager.GetLogs();
        }

        #region Post

        // Crea un nuevo documento
        private async void nuevoDoc_Click(object sender, RoutedEventArgs e)
        {

            #region Datos para testeo 

            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
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

            // Codigo a utilizar
            var respuesta = await consultasAPI.PostDocumentoAsync(documento);

            //Respuestas
            int? nuevodoc = respuesta.Id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());
        }

        // Agrega una nueva relacion de documentos
        private async void nuevoDocRel_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo 

            var documentoRel = new Biblioteca.DocumentoRel
            {
                // Define las propiedades del documento
                SuperiorID = 1,
                InferiorID = 2,
                CuentaID = 3,
                PorInsumos = true
            };

            #endregion


            // Codigo a utilizar
            var respuesta = await consultasAPI.PostDocumentoRelAsync(documentoRel);


            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }
        }

        private async void nuevoAgrupador_Click(object sender, RoutedEventArgs e)
        {

            #region Datos para testeo 

            var agupador = new Agrupador
            {
                // Define las propiedades del documento
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                TipoID = 'B',
                Editado = DateTime.Now,
                Descrip = "a",
                Numero = 1,
                Active = true,
            };

            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.PostAgrupadorAsync(agupador);

            //Respuestas
            int? nuevodoc = respuesta.Id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());

        }

        private async void nuevoDocDet_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo

            var nuevoDocDet = new DocumentoDet
            {
                CuentaID = 1,
                UsuarioID = 1,
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
                Fecha = null,
                ArticuloDescrip = null,
                ArticuloCantSuma = 1,
                ArticuloCantResta = 1,
                ArticuloPrecio = 1,
                SumaPesos = null,
                RestaPesos = null,
                SumaDolares = null,
                RestaDolares = null,
                Cambio = 1

            };

            #endregion

            // Código a utilizar
            var respuesta = await consultasAPI.PostDocumentoDetAsync(nuevoDocDet);

            // Respuestas
            int? nuevoDetalle = respuesta.id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            // Mensaje para testeo
            MessageBox.Show(conexionExitosa + " " + mensaje + " " + nuevoDetalle.ToString());
        }

        #endregion

        #region GET


        //Verifica un usuario, graba el Token y obtiene sus datos
        private async void veriUsu_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña
            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.ValidarUsuarioAsync(email, pass);

            //Respuestas
            Usuario datosusuario = respuesta.Usuario;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Usuario != null)
            {
                MessageBox.Show(respuesta.Usuario.Nombre + " " + respuesta.Usuario.Apellido);
            }

        }

        // Obtiene documentos por ID
        private async void buscaIDDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 19; // ID del documento a obtener
            #endregion

            // Código a utilizar
            var docBuscado = await consultasAPI.ObtenerDocumentoPorID(id);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            Documento? documento = docBuscado.doc;

            //Mensaje para testeo
            if (docBuscado.Success != null)
            {
                MessageBox.Show(docBuscado.Success + " " + docBuscado.Message);
            }

        }

        // Obtiene las relaciones de un documento desde el superiorID
        private async void buscaDocRel_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int superiorID = 221; // ID del documento a obtener
            #endregion

            // Código a utilizar
            var docBuscado = await consultasAPI.GetDocumentosRelPorSupIDAsync(superiorID);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<DocumentoRel> documento = docBuscado.DocumentosRel;

            //Mensaje para testeo
            if (docBuscado.Success == true)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documento.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }
        }


        // Busca los documentos de una cuenta 
        private async void buscaCuentaDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 177; // ID del documento a obtener
            #endregion

            // Código a utilizar
            var docBuscado = await consultasAPI.ObtenerDocumentosPorCuentaID(id);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<Documento> documento = docBuscado.docs;

            //Mensaje para testeo
            if (docBuscado.Success == true)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documento.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }

        }


        // Obtener agrupadores por CuentaID
        private async void buscaAgrupador_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 1; // ID del documento a obtener
            #endregion

            // Código a utilizar
            var docBuscado = await consultasAPI.ObtenerAgrupadoresPorCuentaID(id);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<Agrupador> documento = docBuscado.agrupadores;

            //Mensaje para testeo
            if (docBuscado.Success == true)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documento.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }

        }

        // Obtiene el detalle de un documento, especificando el ID y el nombre del tipo de documento ( FacturaID, RemitoID, etc)
        private async void obtenerDocDet_Click(object sender, RoutedEventArgs e)
        {

            #region Datos para testeo
            int id = 1; // ID del documento a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            #endregion

            // Código a utilizar
            var respuesta = await consultasAPI.GetDocumentosDetPorCampoAsync(id, fieldName);

            // Respuestas
            bool resultado = respuesta.Success;
            string mensaje = respuesta.Message;
            List<DocumentoDet> documentosDet = respuesta.DocumentosDet;

            // Mensaje para testeo
            if (respuesta.Success)
            {
                MessageBox.Show(resultado + " " + mensaje + " Cantidad: " + documentosDet.Count());
            }
            else
            {
                MessageBox.Show("No hay registros");
            }

        }


        #endregion

        #region Put

        // Edita un documento
        private async void editaDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            var documento = new Biblioteca.Documento
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

            // Codigo a utilizar
            var respuesta = await consultasAPI.PutDocumentoAsync(documento);

            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo editó, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }
        }

        // Modifica un detalle de documento, pero si todos los campos de ID de tipo de documento son null, borra el registro
        private async void modificarDocDet_Click(object sender, RoutedEventArgs e)
        {

            #region Datos para testeo
            var modificaDocDet = new DocumentoDet
            {
                ID = 8,
                CuentaID = 1,
                UsuarioID = 1,
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
                Fecha = null,
                ArticuloDescrip = null,
                ArticuloCantSuma = 1,
                ArticuloCantResta = 1,
                ArticuloPrecio = 1,
                SumaPesos = null,
                RestaPesos = null,
                SumaDolares = null,
                RestaDolares = null,
                Cambio = 1

            };
            #endregion

            // Código a utilizar
            var respuesta = await consultasAPI.PutDocumentoDetAsync(modificaDocDet);

            // Respuestas
            bool resultado = respuesta.Success;
            string mensaje = respuesta.Message;

            // Mensaje para testeo
            if (respuesta.Success)
            {
                MessageBox.Show("Registro modificado exitosamente: " + mensaje);
            }
            else
            {
                MessageBox.Show("Error al modificar el documento: " + mensaje);
            }
        }

        #endregion

        #region Delete

        // Borra un documento
        private async void borraDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 13;
            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.DeleteDocumentoAsync(id);

            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }

        }

        //Borra una relación de documentos
        private async void borraDocRel_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int supID = 1;
            int infID = 2;
            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.DeleteDocumentoRelAsync(supID, infID);

            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }


        }

        // Borrar agrupador
        private async void borraAgrupador_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 4;
            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.BorrarAgrupador(id);

            //Respuestas
            bool resultadoBorrado = respuesta.Success;  // true si lo borró, false si no existia el registro
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            if (respuesta.Success != null)
            {
                MessageBox.Show(respuesta.Success + " " + respuesta.Message);
            }

        }


        #endregion


        private void limpiaLogin_Click(object sender, RoutedEventArgs e)
        {
            _queueManager.Logs.Clear();
        }
      
    }
}
