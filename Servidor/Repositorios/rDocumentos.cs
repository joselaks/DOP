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

        //public async Task<IEnumerable<Documento>> ObtenerDocumentosAsync()
        //{
        //    using (var db = new SqlConnection(_connectionString))
        //    {
        //        var documentos = await db.QueryAsync<Documento>("sp_GetDocumentos", commandType: CommandType.StoredProcedure);
        //        return documentos;
        //    }
        //}

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

        public async Task<bool> EliminarDocumentoAsync(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var affectedRows = await db.ExecuteAsync("DocumentosDelete", new { ID = id }, commandType: CommandType.StoredProcedure);
                return affectedRows > 0;
            }


        }

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

        public async Task<bool> ActualizarDocumentoAsync(Documento documento)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters(documento);
                var affectedRows = await db.ExecuteAsync("DocumentosPut", parameters, commandType: CommandType.StoredProcedure);
                return affectedRows > 0;
            }

        }
    }
}
