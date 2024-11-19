using Biblioteca;
using Dapper;
using System.Data;
using System.Data.SqlClient;



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
