using Bibioteca.Clases;
using Biblioteca.DTO;
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

        public async Task<List<GastoDTO>> ListarGastosPorUsuarioAsync(int usuarioID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);

                try
                    {
                    var result = await db.QueryAsync<GastoDTO>(
                        "ListarGastosPorUsuario",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al listar los gastos del usuario {usuarioID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task<ProcesarGastoResult> ProcesarGastoAsync(GastoDTO gasto, List<GastoDetalleDTO> detalles, List<int> presupuestosAfectados)
            {
            using var db = new SqlConnection(_connectionString);

            if (gasto == null) throw new ArgumentNullException(nameof(gasto));
            if (detalles == null) detalles = new List<GastoDetalleDTO>();
            if (gasto.Moneda == '\0')
                throw new ArgumentException("Moneda del gasto es requerida.", nameof(gasto.Moneda));

            var tableDetalles = new DataTable();
            tableDetalles.Columns.Add("ID", typeof(int));
            tableDetalles.Columns.Add("Accion", typeof(char));
            tableDetalles.Columns.Add("GastoID", typeof(int));
            tableDetalles.Columns.Add("CobroID", typeof(int));
            tableDetalles.Columns.Add("UsuarioID", typeof(int));
            tableDetalles.Columns.Add("CuentaID", typeof(int));
            tableDetalles.Columns.Add("TipoID", typeof(char));
            tableDetalles.Columns.Add("PresupuestoID", typeof(int));
            tableDetalles.Columns.Add("InsumoID", typeof(string));
            tableDetalles.Columns.Add("TareaID", typeof(string));
            tableDetalles.Columns.Add("UnicoUso", typeof(bool));
            tableDetalles.Columns.Add("Descrip", typeof(string));
            tableDetalles.Columns.Add("Unidad", typeof(string));
            tableDetalles.Columns.Add("Cantidad", typeof(decimal));
            tableDetalles.Columns.Add("FactorConcepto", typeof(decimal));
            tableDetalles.Columns.Add("PrecioUnitario", typeof(decimal));
            tableDetalles.Columns.Add("Moneda", typeof(char));
            tableDetalles.Columns.Add("TipoCambioD", typeof(decimal));
            tableDetalles.Columns.Add("ArticuloID", typeof(int));
            tableDetalles.Columns.Add("Fecha", typeof(DateTime));

            bool esNuevo = gasto.ID == 0;

            foreach (var d in detalles)
                {
                char accionChar = d.Accion == '\0' ? 'A' : d.Accion;
                char tipoChar = d.TipoID == '\0' ? '0' : d.TipoID;
                char monedaChar = gasto.Moneda;

                object gastoIdObj = esNuevo
                    ? (object)DBNull.Value
                    : (d.GastoID.HasValue ? (object)d.GastoID.Value : DBNull.Value);

                object cobroObj = d.CobroID.HasValue ? (object)d.CobroID.Value : DBNull.Value;

                tableDetalles.Rows.Add(
                    d.ID == 0 ? (object)DBNull.Value : d.ID,
                    accionChar,
                    gastoIdObj,
                    cobroObj,
                    d.UsuarioID,
                    d.CuentaID == 0 ? (object)DBNull.Value : d.CuentaID,
                    tipoChar,
                    d.PresupuestoID.HasValue ? (object)d.PresupuestoID.Value : DBNull.Value,
                    string.IsNullOrEmpty(d.InsumoID) ? (object)DBNull.Value : d.InsumoID,
                    string.IsNullOrEmpty(d.TareaID) ? (object)DBNull.Value : d.TareaID,
                    d.UnicoUso.HasValue ? (object)d.UnicoUso.Value : DBNull.Value,
                    string.IsNullOrEmpty(d.Descrip) ? (object)DBNull.Value : d.Descrip,
                    string.IsNullOrEmpty(d.Unidad) ? (object)DBNull.Value : d.Unidad,
                    d.Cantidad,
                    d.FactorConcepto,
                    d.PrecioUnitario,
                    monedaChar,
                    d.TipoCambioD,
                    d.ArticuloID.HasValue ? (object)d.ArticuloID.Value : DBNull.Value,
                    d.Fecha.HasValue ? (object)d.Fecha.Value : (object)DBNull.Value
                );
                }

            var filasConMonedaNull = tableDetalles.Rows.Cast<DataRow>().Where(r => r.IsNull("Moneda")).ToList();
            if (filasConMonedaNull.Any())
                throw new Exception("Hay filas en el TVP con Moneda = NULL.");

            var parameters = new DynamicParameters();
            parameters.Add("@ID", gasto.ID == 0 ? (object)DBNull.Value : gasto.ID, DbType.Int32);
            parameters.Add("@CuentaID", gasto.CuentaID == 0 ? (object)DBNull.Value : (short)gasto.CuentaID, DbType.Int16);
            parameters.Add("@UsuarioID", gasto.UsuarioID, DbType.Int32);
            parameters.Add("@TipoID", gasto.TipoID, DbType.Byte);
            parameters.Add("@FechaDoc", gasto.FechaDoc, DbType.Date);
            parameters.Add("@FechaCreado", gasto.FechaCreado, DbType.Date);
            parameters.Add("@FechaEditado", gasto.FechaEditado, DbType.Date);
            parameters.Add("@Entidad", gasto.Entidad, DbType.String);
            parameters.Add("@Documento", gasto.Documento, DbType.String);
            parameters.Add("@Descrip", gasto.Descrip, DbType.String);
            parameters.Add("@Notas", gasto.Notas, DbType.String);
            parameters.Add("@Importe", gasto.Importe, DbType.Decimal);
            parameters.Add("@Moneda", gasto.Moneda, DbType.AnsiStringFixedLength, size: 1);
            parameters.Add("@TipoCambioD", gasto.TipoCambioD, DbType.Decimal);

            if (presupuestosAfectados != null && presupuestosAfectados.Any())
                {
                var tablePresupuestos = new DataTable();
                tablePresupuestos.Columns.Add("ID", typeof(int));
                foreach (var pid in presupuestosAfectados.Distinct())
                    tablePresupuestos.Rows.Add(pid);
                parameters.Add("@Presupuestos", tablePresupuestos.AsTableValuedParameter("dbo.TT_IntList"));
                }

            parameters.Add("@Detalles", tableDetalles.AsTableValuedParameter("dbo.TT_GastoDetalle"));

            try
                {
                using var multi = await db.QueryMultipleAsync("ProcesarGasto", parameters, commandType: CommandType.StoredProcedure);

                int documentoId = 0;
                if (!multi.IsConsumed)
                    documentoId = await multi.ReadFirstOrDefaultAsync<int>();

                List<int> presupuestoIds = new();
                if (!multi.IsConsumed)
                    {
                    try { presupuestoIds = (await multi.ReadAsync<int>()).ToList(); }
                    catch { presupuestoIds = new List<int>(); }
                    }

                List<PresupuestoResumen> resumenes = new();
                if (!multi.IsConsumed)
                    {
                    try { resumenes = (await multi.ReadAsync<PresupuestoResumen>()).ToList(); }
                    catch { resumenes = new List<PresupuestoResumen>(); }
                    }

                return new ProcesarGastoResult
                    {
                    DocumentoID = documentoId,
                    PresupuestoIDs = presupuestoIds,
                    Resumenes = resumenes
                    };
                }
            catch (SqlException ex)
                {
                throw new Exception($"Error al procesar el gasto: {ex.Message}", ex);
                }
            }


        public async Task<List<GastoDetalleDTO>> ObtenerDetalleGastoAsync(int GastoID, bool esCobro = false)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@GastoID", GastoID, DbType.Int32);
                parameters.Add("@EsCobro", esCobro ? 1 : 0, DbType.Boolean);

                try
                    {
                    var result = await db.QueryAsync<GastoDetalleDTO>(
                        "ObtenerDetalleGasto",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al listar el detalle del gasto {GastoID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task<ProcesarGastoResult> BorrarGastoAsync(int gastoID, List<int>? presupuestosAfectados = null)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", gastoID, DbType.Int32);

                // Siempre enviar @Presupuestos (TVP), vacío si no hay elementos
                var tablePresupuestos = new DataTable();
                tablePresupuestos.Columns.Add("ID", typeof(int));
                if (presupuestosAfectados != null)
                {
                    foreach (var pid in presupuestosAfectados.Distinct())
                        tablePresupuestos.Rows.Add(pid);
                }
                parameters.Add("@Presupuestos", tablePresupuestos.AsTableValuedParameter("dbo.TT_IntList"));

                try
                {
                    using var multi = await db.QueryMultipleAsync(
                        "BorrarGasto",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    int documentoId = 0;
                    if (!multi.IsConsumed)
                        documentoId = await multi.ReadFirstOrDefaultAsync<int>();

                    List<int> presupuestoIds = new List<int>();
                    if (!multi.IsConsumed)
                    {
                        try { presupuestoIds = (await multi.ReadAsync<int>()).ToList(); }
                        catch { presupuestoIds = new List<int>(); }
                    }

                    List<PresupuestoResumen> resumenes = new List<PresupuestoResumen>();
                    if (!multi.IsConsumed)
                    {
                        try { resumenes = (await multi.ReadAsync<PresupuestoResumen>()).ToList(); }
                        catch { resumenes = new List<PresupuestoResumen>(); }
                    }

                    return new ProcesarGastoResult
                    {
                        DocumentoID = documentoId,
                        PresupuestoIDs = presupuestoIds,
                        Resumenes = resumenes
                    };
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Error al borrar el gasto con ID {gastoID}: {ex.Message}", ex);
                }
            }



        }

        public async Task<(bool Success, string Message)> ProcesarCambiosAsignacionGastosAsync(List<GastoReasignacionDTO> cambios)
            {
            if (cambios == null || cambios.Count == 0)
                return (true, "No hay cambios para procesar.");

            using var db = new SqlConnection(_connectionString);

            // Crear DataTable para el TVP
            var table = new DataTable();
            table.Columns.Add("GastoID", typeof(int));
            table.Columns.Add("NuevoInsumoID", typeof(string));

            foreach (var c in cambios)
                table.Rows.Add(c.GastoID, c.NuevoInsumoID ?? (object)DBNull.Value);

            var parameters = new DynamicParameters();
            parameters.Add("@Reasignaciones", table.AsTableValuedParameter("dbo.GastoReasignacionTableType"));

            try
                {
                // El SP devuelve un result set con Success y Message
                var result = await db.QueryFirstOrDefaultAsync<(int Success, string Message)>(
                    "ProcesarCambiosAsignacionGastos",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (result.Success == 1, result.Message);
                }
            catch (SqlException ex)
                {
                return (false, $"Error SQL: {ex.Message}");
                }
            catch (Exception ex)
                {
                return (false, $"Error: {ex.Message}");
                }
            }

        // DTOs auxiliares para leer los result sets devueltos por el SP
        public class ProcesarGastoResult
            {
            public int DocumentoID { get; set; }
            public List<int> PresupuestoIDs { get; set; } = new List<int>();
            public List<PresupuestoResumen> Resumenes { get; set; } = new List<PresupuestoResumen>();
            }

        public class PresupuestoResumen
            {
            public int PresupuestoID { get; set; }
            public string Moneda { get; set; } = string.Empty;
            public decimal TotalGasto { get; set; }
            public decimal TotalCobro { get; set; }
            }
        }
    }

