using Bibioteca.Clases;
using Biblioteca.DTO;
using Dapper;
using System.Data;
using System.Data.SqlClient;


namespace Servidor.Repositorios
    {
    public class rInsumos
        {
        private readonly string _connectionString;

        public rInsumos(string connectionString)
            {
            _connectionString = connectionString;
            }

        // Obtener las listas de artículos por usuario
        public async Task<List<ArticulosListaDTO>> ObtenerListasArticulosPorUsuarioAsync(int usuarioID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);

                try
                    {
                    var result = await db.QueryAsync<ArticulosListaDTO>(
                        "ObtenerListasArticulos",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al obtener las listas de artículos del usuario {usuarioID}: {ex.Message}", ex);
                    }
                }
            }

        // Obtener los artículos de una lista específica por ListaID
        public async Task<List<ArticuloDTO>> ObtenerArticulosPorListaIDAsync(short listaID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@ListaID", listaID, DbType.Int16);

                try
                    {
                    var result = await db.QueryAsync<ArticuloDTO>(
                        "ObtenerArticulosPorListaID",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al obtener los artículos de la lista {listaID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task<(int NewID, string Mensaje)> CrearNuevaListaArticulosAsync(ArticulosListaDTO dto)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@CuentaID", dto.CuentaID, DbType.Int16);
                parameters.Add("@UsuarioID", dto.UsuarioID, DbType.Int32);
                parameters.Add("@TipoID", dto.TipoID, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@EntidadID", dto.EntidadID, DbType.Int32);
                parameters.Add("@Descrip", dto.Descrip, DbType.AnsiString, size: 50);
                parameters.Add("@Fecha", dto.Fecha.Date, DbType.Date);
                parameters.Add("@Moneda", dto.Moneda, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@Active", dto.Active, DbType.Boolean);

                parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

                await db.ExecuteAsync(
                    "sp_NuevaListaArticulos",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                int newId = parameters.Get<int>("@NewID");
                string mensaje = parameters.Get<string>("@Mensaje");

                return (newId, mensaje);
                }
            }

        public async Task ProcesarArticulosPorListaAsync(int listaID, IEnumerable<ArticuloDTO> articulos)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para el parámetro tipo tabla
                var table = new DataTable();
                table.Columns.Add("Codigo", typeof(string));
                table.Columns.Add("Descrip", typeof(string));
                table.Columns.Add("Unidad", typeof(string));
                table.Columns.Add("Precio", typeof(decimal));

                foreach (var art in articulos)
                    {
                    table.Rows.Add(art.Codigo, art.Descrip, art.Unidad, art.Precio);
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@ListaID", listaID, DbType.Int32);
                parameters.Add("@Articulos", table.AsTableValuedParameter("dbo.ArticuloExceTableType"));

                try
                    {
                    await db.ExecuteAsync(
                        "sp_ProcesarArticulosPorLista",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al procesar los artículos para la lista {listaID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task EliminarArticulosListaYArticulosAsync(int listaID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@ListaID", listaID, DbType.Int32);

                try
                    {
                    await db.ExecuteAsync(
                        "EliminarArticulosListaYArticulos",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al eliminar la lista y sus artículos asociados (ListaID: {listaID}): {ex.Message}", ex);
                    }
                }
            }

        public async Task<List<ArticuloBusquedaDTO>> BuscarArticulosAsync(int usuarioID, string tipoID, string descripBusqueda)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);
                parameters.Add("@TipoID", tipoID, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@DescripBusqueda", descripBusqueda, DbType.AnsiString, size: 65);

                try
                    {
                    var result = await db.QueryAsync<ArticuloBusquedaDTO>(
                        "BusquedaArticulos",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al buscar artículos: {ex.Message}", ex);
                    }
                }
            }

        public async Task EditarArticulosAsync(int listaID, IEnumerable<ArticuloDTO> articulos)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para el parámetro tipo tabla
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("CuentaID", typeof(short));
                table.Columns.Add("UsuarioID", typeof(int));
                table.Columns.Add("ListaID", typeof(short));
                table.Columns.Add("EntidadID", typeof(int));
                table.Columns.Add("TipoID", typeof(string));
                table.Columns.Add("Descrip", typeof(string));
                table.Columns.Add("Unidad", typeof(string));
                table.Columns.Add("UnidadFactor", typeof(decimal));
                table.Columns.Add("Codigo", typeof(string));
                table.Columns.Add("Fecha", typeof(DateTime));
                table.Columns.Add("Precio", typeof(decimal));
                table.Columns.Add("Moneda", typeof(string));
                table.Columns.Add("Nota", typeof(string));
                table.Columns.Add("Accion", typeof(string));

                foreach (var art in articulos)
                    {
                    table.Rows.Add(
                        art.ID,
                        art.CuentaID,
                        art.UsuarioID,
                        art.ListaID,
                        art.EntidadID ?? (object)DBNull.Value,
                        art.TipoID,
                        art.Descrip,
                        art.Unidad,
                        art.UnidadFactor,
                        art.Codigo,
                        art.Fecha,
                        art.Precio,
                        art.Moneda,
                        art.Nota,
                        art.Accion?.ToString()
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@ListaID", listaID, DbType.Int32);
                parameters.Add("@Articulos", table.AsTableValuedParameter("dbo.ArticuloEditarTableType"));

                try
                    {
                    await db.ExecuteAsync(
                        "EditarArticulos",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al editar los artículos para la lista {listaID}: {ex.Message}", ex);
                    }
                }
            }

        }

    // Procesar artículos para una lista específica
    public class ProcesarArticulosPorListaRequest
        {
        public int ListaID { get; set; }
        public List<ArticuloDTO> Articulos { get; set; }
        }

    }
