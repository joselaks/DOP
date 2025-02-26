using Bibioteca.Clases;
using Biblioteca;
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
        private readonly HttpQueueManager _queueManager;

        public Conectores()
        {
            InitializeComponent();
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
            var respuesta = await ConsultasAPI.PostDocumentoAsync(documento);

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
            var respuesta = await ConsultasAPI.PostDocumentoRelAsync(documentoRel);


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
                Numero = "1",
                Active = true,
            };

            #endregion

            // Codigo a utilizar
            var respuesta = await ConsultasAPI.PostAgrupadorAsync(agupador);

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

            var nuevoDocDet = new Biblioteca.DocumentoDet
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
            var respuesta = await ConsultasAPI.PostDocumentoDetAsync(nuevoDocDet);

            // Respuestas
            int? nuevoDetalle = respuesta.id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            // Mensaje para testeo
            MessageBox.Show(conexionExitosa + " " + mensaje + " " + nuevoDetalle.ToString());
        }

        #endregion

        #region Get


        //Verifica un usuario, graba el Token y obtiene sus datos
        private async void veriUsu_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña
            #endregion

            // Codigo a utilizar
            var respuesta = await ConsultasAPI.ValidarUsuarioAsync(email, pass);

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
            var docBuscado = await ConsultasAPI.ObtenerDocumentoPorID(id);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            Biblioteca.Documento? documento = docBuscado.doc;

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
            var docBuscado = await ConsultasAPI.GetDocumentosRelPorSupIDAsync(superiorID);

            // Respuestas
            bool resultado = docBuscado.Success;
            string mensaje = docBuscado.Message;
            List<Biblioteca.DocumentoRel> documento = docBuscado.DocumentosRel;

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
            int id = 1; // ID del usuario
            #endregion

            // Código a utilizar
            var docBuscado = await ConsultasAPI.ObtenerDocumentosPorCuentaID(id);

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
            var docBuscado = await ConsultasAPI.ObtenerAgrupadoresPorCuentaID(id);

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
            int id = 2; // ID del documento a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            short cuentaID = 1; // ID de la cuenta
            #endregion

            // Código a utilizar
            var respuesta = await ConsultasAPI.GetDocumentosDetPorCampoAsync(id, fieldName, cuentaID);

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
            var respuesta = await ConsultasAPI.PutDocumentoAsync(documento);

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
            var respuesta = await ConsultasAPI.PutDocumentoDetAsync(modificaDocDet);

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
            var respuesta = await ConsultasAPI.DeleteDocumentoAsync(id);

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
            var respuesta = await ConsultasAPI.DeleteDocumentoRelAsync(supID, infID);

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
            var respuesta = await ConsultasAPI.BorrarAgrupador(id);

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

        private async void ProcesarDoc_Click(object sender, RoutedEventArgs e)
        {
            // Crear un ejemplo de datos para InfoDocumento
            var documentoDet = new DocumentoDet
            {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 'P',
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
                ArticuloDescrip = "Descripción de prueba 1",
                ArticuloCantSuma = 1,
                ArticuloCantResta = 0,
                ArticuloPrecio = 100,
                SumaPesos = null,
                RestaPesos = null,
                SumaDolares = null,
                RestaDolares = null,
                Cambio = null,
                Accion = 'A'
            };

            var movimiento = new Biblioteca.Movimiento
            {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 1,
                Descrip = "Movimiento de prueba 1",
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                GastoID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 12345,
                Numero = null,
                Notas = "Notas de prueba",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 1,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 100,
                RestaPesos = 0,
                SumaDolares = 0,
                RestaDolares = 0,
                Cambio = 1,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'A'
            };

            var impuesto = new Biblioteca.Impuesto
            {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 1,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = null,
                Descrip = "Impuesto de prueba 1",
                Notas = "Notas del impuesto",
                Previsto = true,
                SumaPesos = 100,
                RestaPesos = 0,
                Alicuota = 21,
                MovimientoID = null,
                Accion = 'M'
            };

            var infoDocumento = new InfoDocumento
            {
                DetalleDocumento = new List<DocumentoDet> { documentoDet },
                DetalleMovimientos = new List<Biblioteca.Movimiento> { movimiento },
                DetalleImpuestos = new List<Biblioteca.Impuesto> { impuesto }
            };

            // Llamar al método ProcesarInfoDocumentoAsync
            var resultado = await ConsultasAPI.ProcesarInfoDocumentoAsync(infoDocumento);

            // Manejar el resultado de la llamada
            if (resultado.Success)
            {
                MessageBox.Show($"Documento procesado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar el documento: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ProcesaLoteDet_Click(object sender, RoutedEventArgs e)
        {
            // Crear tres registros de DocumentoDet
            var documentoDet1 = new Biblioteca.DocumentoDet
            {
                ID = 25,
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
                Accion = 'M'
            };

            var documentoDet2 = new Biblioteca.DocumentoDet
            {
                ID = 26,
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
                Accion = 'D'
            };

            var documentoDet3 = new Biblioteca.DocumentoDet
            {
                ID = 3,
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
                ArticuloDescrip = "Nuevo",
                ArticuloCantSuma = 30,
                ArticuloCantResta = 0,
                ArticuloPrecio = 300,
                SumaPesos = 3000,
                RestaPesos = 0,
                SumaDolares = 30,
                RestaDolares = 0,
                Cambio = 1,
                Accion = 'A'
            };

            // Crear la lista de DocumentoDet
            var listaDetalleDocumento = new List<Biblioteca.DocumentoDet> { documentoDet1, documentoDet2, documentoDet3 };

            // Llamar al método ProcesarListaDetalleDocumentoAsync
            var resultado = await ConsultasAPI.ProcesarListaDetalleDocumentoAsync(listaDetalleDocumento);

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

        private async void ProcesaDetallePres_Click(object sender, RoutedEventArgs e)
        {
            // Crear ejemplos de datos para Conceptos
            var concepto1 = new Concepto
            {
                Codigo = "C001",
                Descrip = "Concepto 1",
                Tipo = "M",
                Unidad = "gl",
                Accion = "A"
            };

            var concepto2 = new Concepto
            {
                Codigo = "C002",
                Descrip = "Concepto 2",
                Tipo = "O",
                Unidad = "kg",
                Accion = "A"
            };

            var concepto3 = new Concepto
            {
                Codigo = "C003",
                Descrip = "Concepto 3",
                Tipo = "A",
                Unidad = "m2",
                Accion = "A"
            };

            // Crear ejemplos de datos para Relaciones
            var relacion1 = new Relacion
            {
                Superior = "S001",
                Inferior = "I001",
                Descrip = "Relación 1",
                Accion = "A"
            };

            var relacion2 = new Relacion
            {
                Superior = "S002",
                Inferior = "I002",
                Descrip = "Relación 2",
                Accion = "A"
            };

            var relacion3 = new Relacion
            {
                Superior = "S003",
                Inferior = "I003",
                Descrip = "Relación 3",
                Accion = "A"
            };

            // Crear la solicitud de procesamiento del árbol de presupuesto
            var request = new ProcesarArbolPresupuestoRequest
            {
                PresupuestoID = 1, // ID de presupuesto de prueba
                ListaConceptos = new List<Concepto> { concepto1, concepto2, concepto3 },
                ListaRelaciones = new List<Relacion> { relacion1, relacion2, relacion3 }
            };

            // Llamar al método ProcesarArbolPresupuestoAsync
            var resultado = await ConsultasAPI.ProcesarArbolPresupuestoAsync(request);

            // Manejar el resultado de la llamada
            if (resultado.Success)
            {
                MessageBox.Show("Árbol de presupuesto procesado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar el árbol de presupuesto: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void obtenerMovimientos_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ProcesaLoteMov_Click(object sender, RoutedEventArgs e)
        {
            // Crear tres registros de Movimiento
            var movimiento1 = new Biblioteca.Movimiento
            {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 1,
                Descrip = "Movimiento de prueba 1",
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                GastoID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 12345,
                Numero = null,
                Notas = "Notas de prueba 1",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 1,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 100,
                RestaPesos = 0,
                SumaDolares = 0,
                RestaDolares = 0,
                Cambio = 1,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'A'
            };

            var movimiento2 = new Biblioteca.Movimiento
            {
                ID = 2,
                CuentaID = 2,
                UsuarioID = 2,
                Editado = DateTime.Now,
                TipoID = 2,
                Descrip = "Movimiento de prueba 2",
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                GastoID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 67890,
                Numero = null,
                Notas = "Notas de prueba 2",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 2,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 200,
                RestaPesos = 0,
                SumaDolares = 0,
                RestaDolares = 0,
                Cambio = 1,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'M'
            };

            var movimiento3 = new Biblioteca.Movimiento
            {
                ID = 3,
                CuentaID = 3,
                UsuarioID = 3,
                Editado = DateTime.Now,
                TipoID = 3,
                Descrip = "Movimiento de prueba 3",
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                GastoID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 11223,
                Numero = null,
                Notas = "Notas de prueba 3",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 3,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 300,
                RestaPesos = 0,
                SumaDolares = 0,
                RestaDolares = 0,
                Cambio = 1,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'D'
            };

            // Crear la lista de Movimiento
            var listaMovimientos = new List<Biblioteca.Movimiento> { movimiento1, movimiento2, movimiento3 };

            // Llamar al método ProcesarMovimientosAsync
            var resultado = await ConsultasAPI.ProcesarMovimientosAsync(listaMovimientos);

            // Manejar el resultado de la llamada
            if (resultado.Success)
            {
                MessageBox.Show("Lista de movimientos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar la lista de movimientos: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void obtenerImpuestos_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ProcesaLoteImp_Click(object sender, RoutedEventArgs e)
        {
            // Crear tres registros de Impuesto
            var impuesto1 = new Biblioteca.Impuesto
            {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 1,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 12345,
                Descrip = "Impuesto de prueba 1",
                Notas = "Notas de prueba 1",
                Previsto = false,
                SumaPesos = 100,
                RestaPesos = 0,
                Alicuota = 21,
                MovimientoID = null,
                Accion = 'A'
            };

            var impuesto2 = new Biblioteca.Impuesto
            {
                ID = 2,
                CuentaID = 2,
                UsuarioID = 2,
                Editado = DateTime.Now,
                TipoID = 2,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 67890,
                Descrip = "Impuesto de prueba 2",
                Notas = "Notas de prueba 2",
                Previsto = false,
                SumaPesos = 200,
                RestaPesos = 0,
                Alicuota = 21,
                MovimientoID = null,
                Accion = 'M'
            };

            var impuesto3 = new Biblioteca.Impuesto
            {
                ID = 3,
                CuentaID = 3,
                UsuarioID = 3,
                Editado = DateTime.Now,
                TipoID = 3,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                EntidadTipo = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                Fecha = DateTime.Now,
                Comprobante = 11223,
                Descrip = "Impuesto de prueba 3",
                Notas = "Notas de prueba 3",
                Previsto = false,
                SumaPesos = 300,
                RestaPesos = 0,
                Alicuota = 21,
                MovimientoID = null,
                Accion = 'D'
            };

            // Crear la lista de Impuesto
            var listaImpuestos = new List<Biblioteca.Impuesto> { impuesto1, impuesto2, impuesto3 };

            // Llamar al método ProcesarImpuestosAsync
            var resultado = await ConsultasAPI.ProcesarImpuestosAsync(listaImpuestos);

            // Manejar el resultado de la llamada
            if (resultado.Success)
            {
                MessageBox.Show("Lista de impuestos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar la lista de impuestos: {resultado.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}

