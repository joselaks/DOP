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
                // Crear DataTable para Detalles (debe coincidir con dbo.TT_GastoDetalle)
                var tableDetalles = new DataTable();
                tableDetalles.Columns.Add("ID", typeof(int));
                tableDetalles.Columns.Add("GastoID", typeof(int));
                tableDetalles.Columns.Add("UsuarioID", typeof(int));
                tableDetalles.Columns.Add("CuentaID", typeof(int));
                tableDetalles.Columns.Add("TipoID", typeof(char));
                tableDetalles.Columns.Add("PresupuestoID", typeof(int));
                tableDetalles.Columns.Add("RubroID", typeof(string));
                tableDetalles.Columns.Add("TareaID", typeof(string));
                tableDetalles.Columns.Add("AuxiliarID", typeof(string));
                tableDetalles.Columns.Add("InsumoID", typeof(string));
                tableDetalles.Columns.Add("Descrip", typeof(string));
                tableDetalles.Columns.Add("Unidad", typeof(string));
                tableDetalles.Columns.Add("Cantidad", typeof(decimal));
                tableDetalles.Columns.Add("FactorCantidad", typeof(decimal)); // ahora usamos la propiedad del DTO
                tableDetalles.Columns.Add("PrecioUnitario", typeof(decimal));
                tableDetalles.Columns.Add("ArticuloID", typeof(int));
                tableDetalles.Columns.Add("MaestroID", typeof(int));
                tableDetalles.Columns.Add("ConceptoMaestroID", typeof(string));
                tableDetalles.Columns.Add("Moneda", typeof(char));
                tableDetalles.Columns.Add("Importe", typeof(decimal));
                tableDetalles.Columns.Add("Accion", typeof(char));

                bool esNuevo = gasto.ID == 0;

                foreach (var d in detalles)
                {
                    tableDetalles.Rows.Add(
                        d.ID == 0 ? (object)DBNull.Value : d.ID,
                        esNuevo ? (object)DBNull.Value : d.GastoID,
                        d.UsuarioID,
                        d.CuentaID == 0 ? (object)DBNull.Value : d.CuentaID,
                        d.TipoID == '\0' ? (object)DBNull.Value : d.TipoID,
                        d.PresupuestoID.HasValue ? (object)d.PresupuestoID.Value : DBNull.Value,
                        string.IsNullOrEmpty(d.RubroID) ? (object)DBNull.Value : d.RubroID,
                        string.IsNullOrEmpty(d.TareaID) ? (object)DBNull.Value : d.TareaID,
                        string.IsNullOrEmpty(d.AuxiliarID) ? (object)DBNull.Value : d.AuxiliarID,
                        string.IsNullOrEmpty(d.InsumoID) ? (object)DBNull.Value : d.InsumoID,
                        string.IsNullOrEmpty(d.Descrip) ? (object)DBNull.Value : d.Descrip,
                        string.IsNullOrEmpty(d.Unidad) ? (object)DBNull.Value : d.Unidad,
                        d.Cantidad,
                        d.FactorCantidad, // uso del nuevo campo
                        d.PrecioUnitario,
                        d.ArticuloID.HasValue ? (object)d.ArticuloID.Value : DBNull.Value,
                        d.MaestroID.HasValue ? (object)d.MaestroID.Value : DBNull.Value,
                        string.IsNullOrEmpty(d.ConceptoMaestroID) ? (object)DBNull.Value : d.ConceptoMaestroID,
                        d.Moneda == '\0' ? (object)DBNull.Value : d.Moneda,
                        d.Importe,
                        d.Accion
                    );
                }

                var parameters = new DynamicParameters();
                parameters.Add("@ID", gasto.ID == 0 ? (object)DBNull.Value : gasto.ID, DbType.Int32);
                parameters.Add("@CuentaID", gasto.CuentaID, DbType.Int32);
                parameters.Add("@UsuarioID", gasto.UsuarioID, DbType.Int32);
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

        public async Task<List<GastoDetalleDTO>> ObtenerDetalleGastoAsync(int GastoID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@GastoID", GastoID, DbType.Int32);

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
