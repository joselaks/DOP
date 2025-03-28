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

            // Llamar al método ValidarUsuarioAsync
            var (success, message, usuario) = await DatosWeb.ValidarUsuarioAsync(email, pass);
            // usuario es un objeto UsuarioDTO que contiene los datos del usuario y el token


            // Manejar la respuesta
            if (success)
                {
                MessageBox.Show(usuario.DatosUsuario.Nombre + " " + usuario.DatosUsuario.Apellido);
                }
            else
                {
                MessageBox.Show($"Error al conectar: {message}");
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

        // Borra un documento
        private async void borraDoc_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int id = 33;
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

        // Edita un documento
        private async void editaDoc_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            var documento = new Biblioteca.DTO.DocumentoDTO
                {
                ID = 21,
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

        private async void buscaCuentaDoc_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int CuentaId = 1; // ID del impuesto a obtener
            #endregion

            // Llamar al método ObtenerDocumentosPorCuentaIDAsync
            var (success, message, documentos) = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(CuentaId);
            // documentos es una lista de DocumentoDTO

            // Manejar la respuesta
            if (success)
                {
                string mensaje = $"Documentos obtenidos exitosamente. Cantidad: {documentos.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los documentos: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        private async void obtenerDocDet_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int id = 303; // ID del documento a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            short cuentaID = 4000; // ID de la cuenta
            #endregion

            // Llamar al método ObtenerDocumentosDetPorCampoAsync
            var (success, message, documentos) = await DatosWeb.ObtenerDocumentosDetPorCampoAsync(id, fieldName, cuentaID);
            // documentos es una lista de DocumentoDetDTO

            // Manejar la respuesta
            if (success)
                {
                string mensaje = $"Documentos obtenidos exitosamente. Cantidad: {documentos.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los documentos: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                ArticuloDescrip = "Artículo 444",
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
                ID = 1,
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
                Accion = 'M'
                };

            var documentoDet3 = new DocumentoDetDTO
                {
                ID = 2,
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
                Accion = 'D'
                };

            // Crear la lista de DocumentoDetDTO
            var listaDetalleDocumento = new List<DocumentoDetDTO> { documentoDet1, documentoDet2, documentoDet3 };

            // Llamar al método ProcesarListaDetalleDocumentoAsync
            var (success, message) = await DatosWeb.ProcesarListaDetalleDocumentoAsync(listaDetalleDocumento);

            // Manejar el resultado de la llamada
            if (success)
                {
                MessageBox.Show("Lista de detalles de documentos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al procesar la lista de detalles de documentos: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }



        private void limpiaLogin_Click(object sender, RoutedEventArgs e)
            {
            DatosWeb.LogEntries.Clear();
            }

        private async void obtenerMovimientos_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int id = 1; // ID del movimiento a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            short cuentaID = 1; // ID de la cuenta
            #endregion

            // Llamar al método ObtenerMovimientosPorCampoAsync
            var (success, message, movimientos) = await DatosWeb.ObtenerMovimientosPorCampoAsync(id, fieldName, cuentaID);
            // movimientos es una lista de Movimiento

            // Manejar la respuesta
            if (success)
                {
                string mensaje = $"Movimientos obtenidos exitosamente. Cantidad: {movimientos.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los movimientos: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        private async void ProcesaLoteMov_Click(object sender, RoutedEventArgs e)
            {
            // Crear tres registros de MovimientoDTO
            var movimiento1 = new MovimientoDTO
                {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                Editado = DateTime.Now,
                TipoID = 1,
                Descrip = "Movimiento 1",
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
                Comprobante = 123,
                Numero = 1,
                Notas = "Notas del movimiento 1",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 1,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 1000,
                RestaPesos = 0,
                SumaDolares = 10,
                RestaDolares = 0,
                Cambio = 1.0m,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'A'
                };

            var movimiento2 = new MovimientoDTO
                {
                ID = 4,
                CuentaID = 2,
                UsuarioID = 2,
                Editado = DateTime.Now,
                TipoID = 2,
                Descrip = "Movimiento 4",
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
                Comprobante = 456,
                Numero = 2,
                Notas = "Notas del movimiento 2",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 1,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 2000,
                RestaPesos = 0,
                SumaDolares = 20,
                RestaDolares = 0,
                Cambio = 1.0m,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'M'
                };

            var movimiento3 = new MovimientoDTO
                {
                ID = 6,
                CuentaID = 3,
                UsuarioID = 3,
                Editado = DateTime.Now,
                TipoID = 3,
                Descrip = "Movimiento 3",
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
                Comprobante = 789,
                Numero = 3,
                Notas = "Notas del movimiento 3",
                ConciliadoFecha = DateTime.Now,
                ConciliadoUsuario = 1,
                ChequeProcesado = false,
                Previsto = false,
                Desdoblado = false,
                SumaPesos = 3000,
                RestaPesos = 0,
                SumaDolares = 30,
                RestaDolares = 0,
                Cambio = 1.0m,
                RelMov = false,
                ImpuestoID = null,
                Accion = 'A'
                };

            // Crear la lista de MovimientoDTO
            var listaMovimientos = new List<MovimientoDTO> { movimiento1, movimiento2, movimiento3 };

            // Llamar al método ProcesarMovimientosAsync
            var (success, message) = await DatosWeb.ProcesarMovimientosAsync(listaMovimientos);

            // Manejar el resultado de la llamada
            if (success)
                {
                MessageBox.Show("Lista de movimientos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al procesar la lista de movimientos: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }




        private async void obtenerImpuestos_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int id = 1; // ID del impuesto a obtener
            string fieldName = "FacturaID"; // Nombre del campo por el cual se va a filtrar
            short cuentaID = 1; // ID de la cuenta
            #endregion

            // Llamar al método ObtenerImpuestosPorCampoAsync
            var (success, message, impuestos) = await DatosWeb.ObtenerImpuestosPorCampoAsync(id, fieldName, cuentaID);
            // impuestos es una lista de Impuesto

            // Manejar la respuesta
            if (success)
                {
                string mensaje = $"Impuestos obtenidos exitosamente. Cantidad: {impuestos.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los impuestos: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private async void ProcesaLoteImp_Click(object sender, RoutedEventArgs e)
            {
            // Crear tres registros de ImpuestoDTO
            var impuesto1 = new ImpuestoDTO
                {
                ID = 1,
                CuentaID = 1,
                UsuarioID = 1,
                TipoID = 1,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                MovimientoID = null,
                Descrip = "Impuesto 1",
                Notas = "Notas del impuesto 1",
                Previsto = false,
                Alicuota = 0.21m,
                Accion = 'A'
                };

            var impuesto2 = new ImpuestoDTO
                {
                ID = 12,
                CuentaID = 2,
                UsuarioID = 2,
                TipoID = 2,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                MovimientoID = null,
                Descrip = "Impuesto 2",
                Notas = "Notas del impuesto 12",
                Previsto = false,
                Alicuota = 0.10m,
                Accion = 'M'
                };

            var impuesto3 = new ImpuestoDTO
                {
                ID = 10,
                CuentaID = 3,
                UsuarioID = 3,
                TipoID = 3,
                TesoreriaID = null,
                AdminID = null,
                ObraID = null,
                EntidadID = null,
                CompraID = null,
                ContratoID = null,
                FacturaID = null,
                OrdenID = null,
                CobroID = null,
                PagoID = null,
                MovimientoID = null,
                Descrip = "Impuesto 3",
                Notas = "Notas del impuesto 3",
                Previsto = false,
                Alicuota = 0.05m,
                Accion = 'D'
                };

            // Crear la lista de ImpuestoDTO
            var listaImpuestos = new List<ImpuestoDTO> { impuesto1, impuesto2, impuesto3 };

            // Llamar al método ProcesarImpuestosAsync
            var (success, message) = await DatosWeb.ProcesarImpuestosAsync(listaImpuestos);

            // Manejar el resultado de la llamada
            if (success)
                {
                MessageBox.Show("Lista de impuestos procesada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al procesar la lista de impuestos: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private async void nuevoAgrupador_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo

            var agrupador = new AgrupadorDTO
                {
                CuentaID = 1,
                UsuarioID = 1,
                TipoID = 'A',
                Editado = DateTime.Now, // Asegurarse de que el campo Editado se inicialice correctamente
                Descrip = "Nuevo Agrupador A",
                Numero = "12345",
                Active = true
                };

            #endregion

            // Llamar al método InsertarAgrupadorAsync
            var (success, message, id) = await DatosWeb.InsertarAgrupadorAsync(agrupador);

            // Manejar la respuesta
            if (success && id.HasValue)
                {
                MessageBox.Show($"Agrupador creado con éxito. ID: {id.Value}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al crear el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private async void obtenerAgrupador_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            short cuentaID = 1; // ID de la cuenta a obtener
            #endregion

            // Llamar al método ObtenerAgrupadoresPorCuentaIDAsync
            var (success, message, agrupadores) = await DatosWeb.ObtenerAgrupadoresPorCuentaIDAsync(cuentaID);

            // Manejar la respuesta
            if (success)
                {
                string mensaje = $"Agrupadores obtenidos exitosamente. Cantidad: {agrupadores.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los agrupadores: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        private async void borrarrAgrupador_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int id = 40; // ID del agrupador a eliminar
            #endregion

            // Llamar al método EliminarAgrupadorAsync
            var (success, message) = await DatosWeb.EliminarAgrupadorAsync(id);

            // Manejar la respuesta
            if (success)
                {
                MessageBox.Show($"Agrupador eliminado con éxito. ID: {id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al eliminar el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }



        private async void editaAgrup_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo

            var agrupador = new AgrupadorDTO
                {
                ID = 40, // ID del agrupador a actualizar
                CuentaID = 1,
                UsuarioID = 1,
                TipoID = 'B',
                Editado = DateTime.Now,
                Descrip = "Agrupador Actualizado B111",
                Numero = "12345",
                Active = true
                };

            #endregion

            // Llamar al método ActualizarAgrupadorAsync
            var (success, message) = await DatosWeb.ActualizarAgrupadorAsync(agrupador);

            // Manejar la respuesta
            if (success)
                {
                MessageBox.Show($"Agrupador actualizado con éxito. ID: {agrupador.ID}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                MessageBox.Show($"Error al actualizar el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private async void obtenerDetallePres_Click(object sender, RoutedEventArgs e)
            {
            #region Datos para testeo
            int presupuestoID = 1; // ID del presupuesto a obtener
            #endregion

            // Llamar al método ObtenerRegistrosPorPresupuestoIDAsync
            var (success, message, data) = await DatosWeb.ObtenerRegistrosPorPresupuestoIDAsync(presupuestoID);

            // Manejar la respuesta
            if (success)
                {
                var (conceptos, relaciones) = data;
                string mensaje = $"Registros obtenidos exitosamente. Cantidad de Conceptos: {conceptos.Count}, Cantidad de Relaciones: {relaciones.Count}";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            else
                {
                string mensaje = $"Error al obtener los registros: {message}";
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        private async void ProcesaDetallePres_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            int presupuestoID = 1; // ID del presupuesto a procesar
            var listaConceptos = new List<ConceptoDTO>
    {
        new ConceptoDTO { PresupuestoID = presupuestoID, Codigo = "C01", Descrip = "Rubro 1", Precio1 = 100, Precio2 = 0, Tipo = 'A', Unidad = "Gl", FechaPrecio = DateTime.Now, Accion = 'A' },
        new ConceptoDTO { PresupuestoID = presupuestoID, Codigo = "C02", Descrip = "Tarea 2", Precio1 = 100, Precio2 = 0, Tipo = 'A', Unidad = "Gl", FechaPrecio = DateTime.Now, Accion = 'A' }
    };

            var listaRelaciones = new List<RelacionDTO>
    {
        new RelacionDTO { PresupuestoID = presupuestoID, Superior = "C01", Inferior = "R03", Cantidad = 200, OrdenInt = 12, Accion = 'A' }
    };
            #endregion

            // Crear el objeto ProcesaPresupuestoDTO
            var request = new ProcesaPresupuestoDTO
            {
                PresupuestoID = presupuestoID,
                ListaConceptos = listaConceptos,
                ListaRelaciones = listaRelaciones
            };

            // Llamar al método ProcesarArbolPresupuestoAsync
            var (success, message) = await DatosWeb.ProcesarArbolPresupuestoAsync(request);

            // Manejar la respuesta
            if (success)
            {
                MessageBox.Show("Árbol de presupuesto procesado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar el árbol de presupuesto: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}

