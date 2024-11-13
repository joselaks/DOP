using Biblioteca;
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
    /// <summary>
    /// Lógica de interacción para Conectores.xaml
    /// </summary>
    public partial class Conectores : Window
    {
        //DatosWeb datosWeb;
        ConsultasAPI consultasAPI;
        private readonly HttpQueueManager _queueManager;

        public Conectores()
        {
            InitializeComponent();
            //datosWeb = new DatosWeb();
            consultasAPI = new ConsultasAPI();
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            this.LogListBox.ItemsSource = _queueManager.Logs;
        }

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
            MessageBox.Show(respuesta.Success + " " + mensaje + " "+ nuevodoc.ToString());
        }

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

        // Borra un documento
        private async void borraDoc_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int id = 22;
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

        // Edita un documento
        private async void editaDoc_Click(object sender, RoutedEventArgs e)
        {
        //    var documento = new Biblioteca.Documento
        //    {
        //        #region
        //        ID = 10,
        //        CuentaID = 55,
        //        TipoID = 5,
        //        UsuarioID = 1,
        //        CreadoFecha = DateTime.Now,
        //        EditadoID = 4,
        //        EditadoFecha = DateTime.Now,
        //        RevisadoID = 5,
        //        RevisadoFecha = DateTime.Now,
        //        AdminID = 3,
        //        ObraID = 5,
        //        PresupuestoID = 6,
        //        RubroID = 6,
        //        EntidadID = 7,
        //        DepositoID = 5,
        //        Descrip = "a",
        //        Concepto1 = "b",
        //        Fecha1 = DateTime.Now,
        //        Fecha2 = DateTime.Now,

        //        Fecha3 = DateTime.Now,
        //        Numero1 = 0,
        //        Numero2 = 0,
        //        Numero3 = 0,
        //        Notas = "bb",
        //        Active = false,
        //        Pesos = 0,
        //        Dolares = 0,
        //        Impuestos = 0,
        //        ImpuestosD = 0,
        //        Materiales = 0,
        //        ManodeObra = 0,
        //        Subcontratos = 0,
        //        Equipos = 0,
        //        Otros = 0,
        //        MaterialesD = 0,
        //        ManodeObraD = 0,
        //        SubcontratosD = 0,
        //        EquiposD = 0,
        //        OtrosD = 0,
        //        RelDoc = false,
        //        RelArt = false,
        //        RelMov = false,
        //        RelImp = false,
        //        RelRub = false,
        //        RelTar = false,
        //        RelIns = false
        //        #endregion
        //    };
        //    var (success, message) = await datosWeb.PutDocumentoAsync(documento);
        //    MessageBox.Show(message, success ? "Éxito" : "Error");
        //}

        //// Busca un documento por su ID
        //private async void buscaIDDoc_Click(object sender, RoutedEventArgs e)
        //{
        //    int id = 3; // ID del documento a obtener
        //    var (success, message, documento) = await datosWeb.ObtenerDocumentoPorIDAsync(id);

        //    if (success)
        //    {
        //        MessageBox.Show(message, "Éxito");
        //        // Aquí se puede mostrar los detalles del documento en la interfaz de usuario
        //        // Por ejemplo:
        //        // textBoxDescripcion.Text = documento.Descrip;
        //    }
        //    else
        //    {
        //        MessageBox.Show(message, "Error");
        //    }


        }

        // Busca los documentos de una cuenta 
        private async void buscaCuentaDoc_Click(object sender, RoutedEventArgs e)
        {
            short cuentaID = 1; // ID de la cuenta a consultar
            var docs = await consultasAPI.GetDocumentosPorCuentaIDAsync(cuentaID);
            if (docs.Documentos!=null)
            {
                MessageBox.Show(docs.Success.ToString() + " " + docs.Documentos.Count().ToString());
                // Aquí se puede obtener la lista de documentos
                // Por ejemplo:
                // foreach (var item in docs.Documentos)
                // {
                //     
                // }
            }

        }

        private void limpiaLogin_Click(object sender, RoutedEventArgs e)
        {
            _queueManager.Logs.Clear();
        }

        private void buscaIDDoc_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
