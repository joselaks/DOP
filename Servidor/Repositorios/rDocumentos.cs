using Biblioteca;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;



namespace Servidor.Repositorios
{

    public class rDocumentos
    {
        private readonly string _connectionString;

        public rDocumentos(string connectionString)
        {
            _connectionString = connectionString;
        }


        #region Post

        // Nuevo documento (encabezado)
        public async Task<int> InsertarDocumentoAsync(Documento documento)
        {
            int respuesta = 0;
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(documento);
                parameters.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosPost", parameters, commandType: CommandType.StoredProcedure);
                respuesta = parameters.Get<int>("@ID");

            }
            return respuesta;
        }

        //Procesar lote de Detalles de documentos
        public async Task ProcesarListaDetalleDocumentoAsync(List<DocumentoDet> listaDetalleDocumento)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("CuentaID", typeof(short));
                table.Columns.Add("UsuarioID", typeof(int));
                table.Columns.Add("Editado", typeof(DateTime));
                table.Columns.Add("TipoID", typeof(char));
                table.Columns.Add("AdminID", typeof(int));
                table.Columns.Add("EntidadID", typeof(int));
                table.Columns.Add("DepositoID", typeof(int));
                table.Columns.Add("AcopioID", typeof(int));
                table.Columns.Add("PedidoID", typeof(int));
                table.Columns.Add("CompraID", typeof(int));
                table.Columns.Add("ContratoID", typeof(int));
                table.Columns.Add("FacturaID", typeof(int));
                table.Columns.Add("RemitoID", typeof(int));
                table.Columns.Add("ParteID", typeof(int));
                table.Columns.Add("ObraID", typeof(int));
                table.Columns.Add("PresupuestoID", typeof(int));
                table.Columns.Add("RubroID", typeof(int));
                table.Columns.Add("TareaID", typeof(int));
                table.Columns.Add("Fecha", typeof(DateTime));
                table.Columns.Add("ArticuloDescrip", typeof(string));
                table.Columns.Add("ArticuloCantSuma", typeof(decimal));
                table.Columns.Add("ArticuloCantResta", typeof(decimal));
                table.Columns.Add("ArticuloPrecio", typeof(decimal));
                table.Columns.Add("SumaPesos", typeof(decimal));
                table.Columns.Add("RestaPesos", typeof(decimal));
                table.Columns.Add("SumaDolares", typeof(decimal));
                table.Columns.Add("RestaDolares", typeof(decimal));
                table.Columns.Add("Cambio", typeof(decimal));
                table.Columns.Add("Accion", typeof(char));

                foreach (var item in listaDetalleDocumento)
                {
                    table.Rows.Add(
                        item.ID, item.CuentaID, item.UsuarioID, item.Editado, item.TipoID, item.AdminID,
                        item.EntidadID, item.DepositoID, item.AcopioID, item.PedidoID, item.CompraID,
                        item.ContratoID, item.FacturaID, item.RemitoID, item.ParteID, item.ObraID,
                        item.PresupuestoID, item.RubroID, item.TareaID, item.Fecha, item.ArticuloDescrip,
                        item.ArticuloCantSuma, item.ArticuloCantResta, item.ArticuloPrecio, item.SumaPesos,
                        item.RestaPesos, item.SumaDolares, item.RestaDolares, item.Cambio, item.Accion
                    );
                }

                var parameters = new DynamicParameters();
                parameters.Add("@ListaDetalleDocumento", table.AsTableValuedParameter("dbo.DocumentoDet"));

                try
                {
                    await db.ExecuteAsync("ProcesaListaDetalleDocumento", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Error al procesar la lista de detalles de documentos: {ex.Message}", ex);
                }
            }
        }




        //Procesa el documento completo
        public async Task ProcesarInfoDocumentoAsync(InfoDocumento documento)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                // Serializar las listas a JSON
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                string jsonDetalleDocumento = documento.DetalleDocumento != null ? JsonSerializer.Serialize(documento.DetalleDocumento, jsonOptions) : null;
                string jsonDetalleMovimientos = documento.DetalleMovimientos != null ? JsonSerializer.Serialize(documento.DetalleMovimientos, jsonOptions) : null;
                string jsonDetalleImpuestos = documento.DetalleImpuestos != null ? JsonSerializer.Serialize(documento.DetalleImpuestos, jsonOptions) : null;

                // Agregar parámetros
                parameters.Add("@JsonDetalleDocumento", jsonDetalleDocumento, DbType.String);
                parameters.Add("@JsonDetalleMovimientos", jsonDetalleMovimientos, DbType.String);
                parameters.Add("@JsonDetalleImpuestos", jsonDetalleImpuestos, DbType.String);
                parameters.Add("@MensajeError", dbType: DbType.String, direction: ParameterDirection.Output, size: 4000);

                try
                {
                    // Ejecutar el procedimiento almacenado
                    await db.ExecuteAsync("ProcesarInfoDocumento", parameters, commandType: CommandType.StoredProcedure);

                    // Verificar si hay un mensaje de error
                    string mensajeError = parameters.Get<string>("@MensajeError");
                    if (!string.IsNullOrEmpty(mensajeError))
                    {
                        throw new Exception(mensajeError);
                    }
                }
                catch (SqlException ex)
                {
                    // Capturar el mensaje de error del procedimiento almacenado
                    string mensajeError = parameters.Get<string>("@MensajeError");
                    if (!string.IsNullOrEmpty(mensajeError))
                    {
                        throw new Exception(mensajeError, ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }






        // Nueva Relación documentoRel
        public async Task<bool> InsertarDocumentoRelAsync(DocumentoRel rel)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(rel);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosRelPost", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }


        // Nuevo Agrupador
        public async Task<int> InsertarAgrupadorAsync(AgrupadorAPI agrupador)
        {
            int respuesta = 0;
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(agrupador);
                parameters.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("AgrupadoresPost", parameters, commandType: CommandType.StoredProcedure);
                respuesta = parameters.Get<int>("@ID");

            }
            return respuesta;
        }

        public async Task<int> InsertarDocumentoDetAsync(DocumentoDet documentoDet)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(documentoDet);
                parameters.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosDetPost", parameters, commandType: CommandType.StoredProcedure);
                int id = parameters.Get<int>("@ID");

                return id;
            }
        }


        #endregion

        #region Get

        // Obtener encabezado de documento por cuenta
        public async Task<IEnumerable<Documento>> ObtenerDocumentosPorCuentaIDAsync(int cuentaID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var documentos = await db.QueryAsync<Documento>(
                    "DocumentosGetCuenta",
                    new { CuentaID = cuentaID },
                    commandType: CommandType.StoredProcedure
                );
                return documentos;
            }
        }

        // Obtener documento por ID
        public async Task<Documento> ObtenerDocumentosPorIDAsync(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var documento = await db.QueryFirstAsync<Documento>(
                    "DocumentosGetID",
                    new { ID = id },
                    commandType: CommandType.StoredProcedure
                );
                return documento;
            }
        }

        // Obtener relacion de documento 
        public async Task<IEnumerable<DocumentoRel>> ObtenerDocumentosRelPorSuperiorIDAsync(int superiorID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var documentosRel = await db.QueryAsync<DocumentoRel>(
                    "DocumentosRelGetBySuperiorID",
                    new { SuperiorID = superiorID },
                    commandType: CommandType.StoredProcedure
                );
                return documentosRel;
            }
        }

        // Obtener agrupadores por cuenta
        public async Task<IEnumerable<AgrupadorAPI>> ObtenerAgrupadorPorCuentaIDAsync(int cuentaID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var documentos = await db.QueryAsync<AgrupadorAPI>(
                    "AgrupadoresGetCuenta",
                    new { CuentaID = cuentaID },
                    commandType: CommandType.StoredProcedure
                );
                return documentos;
            }
        }

        public async Task<IEnumerable<DocumentoDet>> ObtenerDocumentosDetPorCampoAsync(int id, string fieldName)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", id, DbType.Int32);
                parameters.Add("@FieldName", fieldName, DbType.String);

                var documentosDet = await db.QueryAsync<DocumentoDet>(
                    "DocumentosDetGetDocumentoID",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return documentosDet;
            }
        }


        #endregion

        #region Put

        public async Task<bool> ActualizarDocumentoAsync(Documento documento)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(documento);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosPut", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }

        public async Task<bool> ActualizarAgrupadorAsync(AgrupadorAPI agrupador)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(agrupador);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("AgrupadoresPut", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }

        public async Task<string> ActualizarOEliminarDocumentoDetAsync(DocumentoDet documentoDet)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(documentoDet);
                parameters.Add("@Mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);

                await db.ExecuteAsync("DocumentosDetPut", parameters, commandType: CommandType.StoredProcedure);

                string mensaje = parameters.Get<string>("@Mensaje");
                return mensaje;
            }
        }




        #endregion

        #region Delete

        //Borrar un encabezado de documento
        public async Task<bool> EliminarDocumentoAsync(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int32);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosDelete", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }

        //Borrar una relación de documento
        public async Task<bool> EliminarDocumentoRelAsync(int superiorID, int inferiorID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SuperiorID", superiorID, DbType.Int32);
                parameters.Add("@@InferiorID", inferiorID, DbType.Int32);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("DocumentosRelDelete", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }

        //Borrar Agrupador
        public async Task<bool> EliminarAgrupadorAsync(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int32);
                parameters.Add("Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await db.ExecuteAsync("AgrupadoresDelete", parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("Success");
                return success;
            }
        }

        #endregion

    }
}
