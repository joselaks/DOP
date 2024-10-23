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
        DatosWeb datosWeb;

        public Conectores()
        {
            InitializeComponent();
            datosWeb = new DatosWeb();
        }

        // Crea un nuevo documento
        private async void nuevoDoc_Click(object sender, RoutedEventArgs e)
        {
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

            var (success, message) = await datosWeb.PostDocumentoAsync(documento);

            MessageBox.Show(message, success ? "Éxito" : "Error");
        }

        //Verifica un usuario, graba el Token y obtiene sus datos
        private async void veriUsu_Click(object sender, RoutedEventArgs e)
        {
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña

            var (success, message, usuario) = await datosWeb.ValidarUsuarioAsync(email, pass);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí puedes mostrar los detalles del usuario en la interfaz de usuario
                // Por ejemplo:
                // textBoxNombre.Text = usuario.Nombre;
            }
            else
            {
                MessageBox.Show(message, "Error");
            }


        }

        // Borra un documento
        private async void borraDoc_Click(object sender, RoutedEventArgs e)
        {
            int id = 10; // ID del documento a eliminar
            var (success, message) = await datosWeb.DeleteDocumentoAsync(id);
            MessageBox.Show(message, success ? "Éxito" : "Error");

        }

        //Edita un documento
        private async void editaDoc_Click(object sender, RoutedEventArgs e)
        {
            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
                ID = 10,
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
            var (success, message) = await datosWeb.PutDocumentoAsync(documento);
            MessageBox.Show(message, success ? "Éxito" : "Error");

        }

        // Busca un documento por su ID
        private async void buscaIDDoc_Click(object sender, RoutedEventArgs e)
        {
            int id = 3; // ID del documento a obtener
            var (success, message, documento) = await datosWeb.ObtenerDocumentoPorIDAsync(id);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí se puede mostrar los detalles del documento en la interfaz de usuario
                // Por ejemplo:
                // textBoxDescripcion.Text = documento.Descrip;
            }
            else
            {
                MessageBox.Show(message, "Error");
            }


        }

        // Busca los documentos de una cuenta 
        private async void buscaCuentaDoc_Click(object sender, RoutedEventArgs e)
        {
            short cuentaID = 5; // ID de la cuenta a consultar
            var (success, message, documentos) = await datosWeb.GetDocumentosPorCuentaIDAsync(cuentaID);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí se puede obtener la lista de documentos
                // Por ejemplo:
                // foreach (var doc in documentos)
                // {
                //     listBoxDocumentos.Items.Add(doc.Descrip);
                // }
            }
            else
            {
                MessageBox.Show(message, "Error");



            }
        }
    }
}
