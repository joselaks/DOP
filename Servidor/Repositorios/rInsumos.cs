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

        // Obtener la lista de insumos por usuario
        public async Task<List<InsumoDTO>> ListarInsumosPorUsuarioAsync(int usuarioID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);

                try
                    {
                    var result = await db.QueryAsync<InsumoDTO>(
                        "ObtenerInsumosPorUsuario",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al listar los insumos del usuario {usuarioID}: {ex.Message}", ex);
                    }
                }
            }

        // Procesar un insumo y sus artículos relacionados
        public async Task<int> ProcesarInsumoAsync(InsumoDTO insumo, List<ArticuloRelDTO> articulos)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para ArticulosRel
                var tableArticulos = new DataTable();
                tableArticulos.Columns.Add("ArticuloID", typeof(int));
                tableArticulos.Columns.Add("InsumoID", typeof(int));
                tableArticulos.Columns.Add("CuentaID", typeof(short));
                tableArticulos.Columns.Add("FactorPrecio", typeof(decimal));
                tableArticulos.Columns.Add("FactorUnidad", typeof(decimal));
                tableArticulos.Columns.Add("Seleccionado", typeof(bool));
                tableArticulos.Columns.Add("Accion", typeof(char));

                foreach (var a in articulos)
                    {
                    tableArticulos.Rows.Add(
                        a.ArticuloID,
                        a.InsumoID,
                        a.CuentaID,
                        a.FactorPrecio,
                        a.FactorUnidad,
                        a.Seleccionado,
                        a.Accion.HasValue ? a.Accion.Value : DBNull.Value
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@ID", insumo.ID == 0 ? (object)DBNull.Value : insumo.ID, DbType.Int32);
                parameters.Add("@CuentaID", insumo.CuentaID, DbType.Int16);
                parameters.Add("@UsuarioID", insumo.UsuarioID, DbType.Int32);
                parameters.Add("@Editado", insumo.Editado, DbType.Date);
                parameters.Add("@Tipo", insumo.Tipo, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@Descrip", insumo.Descrip, DbType.String);
                parameters.Add("@Unidad", insumo.Unidad, DbType.AnsiStringFixedLength, size: 2);
                parameters.Add("@Moneda", insumo.Moneda, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@Precio", insumo.Precio, DbType.Decimal);
                parameters.Add("@Codigo", insumo.Codigo, DbType.String);
                parameters.Add("@ArticulosRel", insumo.ArticulosRel, DbType.Boolean);
                parameters.Add("@Articulos", tableArticulos.AsTableValuedParameter("dbo.TT_ArticulosRel"));

                try
                    {
                    // El procedimiento devuelve el ID del insumo afectado
                    var result = await db.QueryFirstOrDefaultAsync<int>(
                        "ProcesarInsumo",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result;
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al procesar el insumo: {ex.Message}", ex);
                    }
                }
            }

        // Eliminar un insumo y sus artículos relacionados
        public async Task EliminarInsumoYArticulosRelAsync(int insumoID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@InsumoID", insumoID, DbType.Int32);

                try
                    {
                    await db.ExecuteAsync(
                        "EliminarInsumoYArticulosRel",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al eliminar el insumo y sus artículos relacionados (ID: {insumoID}): {ex.Message}", ex);
                    }
                }
            }

        // Procesar una lista de artículos con sus artículos relacionados
        public async Task<int> ProcesarArticulosListaAsync(ArticulosListaDTO lista, List<ArticuloDTO> articulos)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para Articulos
                var tableArticulos = new DataTable();
                tableArticulos.Columns.Add("ID", typeof(int));
                tableArticulos.Columns.Add("CuentaID", typeof(short));
                tableArticulos.Columns.Add("UsuarioID", typeof(int));
                tableArticulos.Columns.Add("ListaID", typeof(short));
                tableArticulos.Columns.Add("EntidadID", typeof(int));
                tableArticulos.Columns.Add("TipoID", typeof(string));
                tableArticulos.Columns.Add("Descrip", typeof(string));
                tableArticulos.Columns.Add("Unidad", typeof(string));
                tableArticulos.Columns.Add("UnidadFactor", typeof(decimal));
                tableArticulos.Columns.Add("Codigo", typeof(string));
                tableArticulos.Columns.Add("Fecha", typeof(DateTime));
                tableArticulos.Columns.Add("Precio", typeof(decimal));
                tableArticulos.Columns.Add("Moneda", typeof(string));
                tableArticulos.Columns.Add("Accion", typeof(char));

                foreach (var a in articulos)
                    {
                    tableArticulos.Rows.Add(
                        a.ID,
                        a.CuentaID,
                        a.UsuarioID,
                        a.ListaID,
                        a.EntidadID.HasValue ? (object)a.EntidadID.Value : DBNull.Value,
                        a.TipoID,
                        a.Descrip,
                        a.Unidad,
                        a.UnidadFactor,
                        a.Codigo,
                        a.Fecha,
                        a.Precio,
                        a.Moneda,
                        a.Accion.HasValue ? a.Accion.Value : DBNull.Value
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@ID", lista.ID == 0 ? (object)DBNull.Value : lista.ID, DbType.Int32);
                parameters.Add("@CuentaID", lista.CuentaID, DbType.Int16);
                parameters.Add("@UsuarioID", lista.UsuarioID, DbType.Int32);
                parameters.Add("@TipoID", lista.TipoID, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@EntidadID", lista.EntidadID.HasValue ? (object)lista.EntidadID.Value : DBNull.Value, DbType.Int32);
                parameters.Add("@Descrip", lista.Descrip, DbType.String);
                parameters.Add("@Fecha", lista.Fecha, DbType.Date);
                parameters.Add("@Moneda", lista.Moneda, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@Active", lista.Active, DbType.Boolean);
                parameters.Add("@Articulos", tableArticulos.AsTableValuedParameter("dbo.TT_Articulos"));

                try
                    {
                    // El procedimiento devuelve el ID de la lista afectada
                    var result = await db.QueryFirstOrDefaultAsync<int>(
                        "ProcesarArticulosLista",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result;
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al procesar la lista de artículos: {ex.Message}", ex);
                    }
                }
            }

        // Eliminar una lista de artículos y sus artículos relacionados
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
                    throw new Exception($"Error al eliminar la lista y sus artículos relacionados (ListaID: {listaID}): {ex.Message}", ex);
                    }
                }
            }

        // Obtener artículos relacionados a un insumo específico
        public async Task<List<ArticuloDTO>> ObtenerArticulosPorInsumoAsync(int insumoID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@InsumoID", insumoID, DbType.Int32);

                try
                    {
                    var result = await db.QueryAsync<ArticuloDTO>(
                        "ObtenerArticulosPorInsumo",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al obtener los artículos del insumo {insumoID}: {ex.Message}", ex);
                    }
                }
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


        }

    }
