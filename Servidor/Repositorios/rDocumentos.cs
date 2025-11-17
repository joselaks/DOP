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

        public async Task<int> ProcesarGastoAsync(GastoDTO gasto, List<GastoDetalleDTO> detalles)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                if (gasto == null) throw new ArgumentNullException(nameof(gasto));
                if (detalles == null) detalles = new List<GastoDetalleDTO>();
                if (gasto.Moneda == '\0')
                    throw new ArgumentException("Moneda del gasto es requerida.", nameof(gasto.Moneda));

                // Crear DataTable para Detalles (orden y nombres deben coincidir exactamente con dbo.TT_GastoDetalle)
                var tableDetalles = new DataTable();
                tableDetalles.Columns.Add("ID", typeof(int));
                tableDetalles.Columns.Add("GastoID", typeof(int));
                tableDetalles.Columns.Add("CobroID", typeof(int));
                tableDetalles.Columns.Add("UsuarioID", typeof(int));
                tableDetalles.Columns.Add("CuentaID", typeof(int));
                tableDetalles.Columns.Add("TipoID", typeof(char));                // CHAR(1)
                tableDetalles.Columns.Add("PresupuestoID", typeof(int));
                tableDetalles.Columns.Add("RubroID", typeof(string));
                tableDetalles.Columns.Add("TareaID", typeof(string));
                tableDetalles.Columns.Add("AuxiliarID", typeof(string));
                tableDetalles.Columns.Add("InsumoID", typeof(string));
                tableDetalles.Columns.Add("Descrip", typeof(string));
                tableDetalles.Columns.Add("Unidad", typeof(string));
                tableDetalles.Columns.Add("Cantidad", typeof(decimal));
                tableDetalles.Columns.Add("FactorCantidad", typeof(decimal));
                tableDetalles.Columns.Add("PrecioUnitario", typeof(decimal));
                tableDetalles.Columns.Add("Moneda", typeof(char));                // CHAR(1)
                tableDetalles.Columns.Add("TipoCambioD", typeof(decimal));
                tableDetalles.Columns.Add("ArticuloID", typeof(int));
                tableDetalles.Columns.Add("MaestroID", typeof(int));
                tableDetalles.Columns.Add("ConceptoMaestroID", typeof(string));
                tableDetalles.Columns.Add("Fecha", typeof(DateTime));
                tableDetalles.Columns.Add("Accion", typeof(char));                // CHAR(1)

                bool esNuevo = gasto.ID == 0;

                foreach (var d in detalles)
                    {
                    // Normalizar valores obligatorios como char
                    char tipoChar = d.TipoID == '\0' ? '0' : d.TipoID;
                    char monedaChar = gasto.Moneda;
                    char accionChar = d.Accion;
                    object cobroObj = d.CobroID.HasValue ? (object)d.CobroID.Value : DBNull.Value;

                    tableDetalles.Rows.Add(
                        d.ID == 0 ? (object)DBNull.Value : d.ID,
                        esNuevo ? (object)DBNull.Value : d.GastoID,
                        cobroObj,
                        d.UsuarioID,
                        d.CuentaID == 0 ? (object)DBNull.Value : d.CuentaID,
                        tipoChar,
                        d.PresupuestoID.HasValue ? (object)d.PresupuestoID.Value : DBNull.Value,
                        string.IsNullOrEmpty(d.RubroID) ? (object)DBNull.Value : d.RubroID,
                        string.IsNullOrEmpty(d.TareaID) ? (object)DBNull.Value : d.TareaID,
                        string.IsNullOrEmpty(d.AuxiliarID) ? (object)DBNull.Value : d.AuxiliarID,
                        string.IsNullOrEmpty(d.InsumoID) ? (object)DBNull.Value : d.InsumoID,
                        string.IsNullOrEmpty(d.Descrip) ? (object)DBNull.Value : d.Descrip,
                        string.IsNullOrEmpty(d.Unidad) ? (object)DBNull.Value : d.Unidad,
                        d.Cantidad,
                        d.FactorCantidad,
                        d.PrecioUnitario,
                        monedaChar,
                        d.TipoCambioD,
                        d.ArticuloID.HasValue ? (object)d.ArticuloID.Value : DBNull.Value,
                        d.MaestroID.HasValue ? (object)d.MaestroID.Value : DBNull.Value,
                        string.IsNullOrEmpty(d.ConceptoMaestroID) ? (object)DBNull.Value : d.ConceptoMaestroID,
                        d.Fecha.HasValue ? (object)d.Fecha.Value : (object)DBNull.Value,
                        accionChar
                    );
                    }

                // Comprobación rápida antes de llamar al SP
                var filasConMonedaNull = tableDetalles.Rows.Cast<DataRow>().Where(r => r.IsNull("Moneda")).ToList();
                if (filasConMonedaNull.Any())
                    throw new Exception("Hay filas en el TVP con Moneda = NULL. Revisa los detalles antes de llamar al procedimiento.");

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
                parameters.Add("@Detalles", tableDetalles.AsTableValuedParameter("dbo.TT_GastoDetalle"));

                try
                    {
                    var result = await db.QueryFirstOrDefaultAsync<int>(
                        "ProcesarGasto",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result;
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al procesar el gasto: {ex.Message}", ex);
                    }
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
    }
}
