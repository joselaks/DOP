using Bibioteca.Clases;
using Biblioteca.DTO;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Servidor.Repositorios
    {
    public class rPresupuestos
        {
        private readonly string _connectionString;

        public rPresupuestos(string connectionString)
            {
            _connectionString = connectionString;
            }
        
        public async Task<List<PresupuestoDTO>> ListarPresupuestosPorUsuarioAsync(int usuarioID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);

                try
                    {
                    var result = await db.QueryAsync<PresupuestoDTO>(
                        "ListarPresupuestosPorUsuario",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result.ToList();
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al listar los presupuestos del usuario {usuarioID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task<int> ProcesarPresupuestoAsync(PresupuestoDTO presupuesto, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones)
            {
            using var db = new SqlConnection(_connectionString);

            // DataTable para Conceptos (esquema definitivo)
            var tableConceptos = new DataTable();
            tableConceptos.Columns.Add("PresupuestoID", typeof(int));
            tableConceptos.Columns.Add("ConceptoID", typeof(string));
            tableConceptos.Columns.Add("Descrip", typeof(string));
            tableConceptos.Columns.Add("Tipo", typeof(char));
            tableConceptos.Columns.Add("Unidad", typeof(string));
            tableConceptos.Columns.Add("PrEjec", typeof(decimal));
            tableConceptos.Columns.Add("PrEjec1", typeof(decimal));
            tableConceptos.Columns.Add("EjecMoneda", typeof(char));
            tableConceptos.Columns.Add("MesBase", typeof(DateTime));
            tableConceptos.Columns.Add("CanTotalEjec", typeof(decimal));
            tableConceptos.Columns.Add("CantTotalReal", typeof(decimal));
            tableConceptos.Columns.Add("Existencias", typeof(decimal));
            tableConceptos.Columns.Add("PrReal", typeof(decimal));
            tableConceptos.Columns.Add("PrReal1", typeof(decimal));
            tableConceptos.Columns.Add("ArticuloID", typeof(int));
            tableConceptos.Columns.Add("FactorArticulo", typeof(decimal));
            tableConceptos.Columns.Add("Accion", typeof(char));

            bool esNuevo = !presupuesto.ID.HasValue || presupuesto.ID == 0;

            foreach (var c in conceptos)
                {
                object presupuestoIdObj = esNuevo ? (object)DBNull.Value : c.PresupuestoID;
                tableConceptos.Rows.Add(
                    presupuestoIdObj,
                    c.ConceptoID ?? string.Empty,
                    c.Descrip ?? string.Empty,
                    c.Tipo,
                    c.Unidad ?? string.Empty,
                    c.PrEjec,
                    c.PrEjec1,
                    c.EjecMoneda,
                    c.MesBase,
                    c.CanTotalEjec,
                    c.CantTotalReal.HasValue ? (object)c.CantTotalReal.Value : DBNull.Value,
                    c.Existencias.HasValue ? (object)c.Existencias.Value : DBNull.Value,
                    c.PrReal.HasValue ? (object)c.PrReal.Value : DBNull.Value,
                    c.PrReal1.HasValue ? (object)c.PrReal1.Value : DBNull.Value,
                    c.ArticuloID.HasValue ? (object)c.ArticuloID.Value : DBNull.Value,
                    c.FactorArticulo.HasValue ? (object)c.FactorArticulo.Value : DBNull.Value,
                    c.Accion.HasValue ? (object)c.Accion.Value : DBNull.Value
                );
                }

            // DataTable para Relaciones (sin CanVenta)
            var tableRelaciones = new DataTable();
            tableRelaciones.Columns.Add("PresupuestoID", typeof(int));
            tableRelaciones.Columns.Add("CodSup", typeof(string));
            tableRelaciones.Columns.Add("CodInf", typeof(string));
            tableRelaciones.Columns.Add("CanEjec", typeof(decimal));
            tableRelaciones.Columns.Add("CanReal", typeof(decimal));
            tableRelaciones.Columns.Add("OrdenInt", typeof(short));
            tableRelaciones.Columns.Add("Accion", typeof(char));

            foreach (var r in relaciones)
                {
                tableRelaciones.Rows.Add(
                    esNuevo ? (object)DBNull.Value : r.PresupuestoID,
                    r.CodSup ?? string.Empty,
                    r.CodInf ?? string.Empty,
                    r.CanEjec,
                    r.CanReal.HasValue ? (object)r.CanReal.Value : DBNull.Value,
                    r.OrdenInt,
                    r.Accion.HasValue ? (object)r.Accion.Value : DBNull.Value
                );
                }

            var parameters = new DynamicParameters();
            parameters.Add("@ID", !presupuesto.ID.HasValue || presupuesto.ID == 0 ? (object)DBNull.Value : presupuesto.ID, DbType.Int32);
            parameters.Add("@CuentaID", presupuesto.CuentaID, DbType.Int32);
            parameters.Add("@UsuarioID", presupuesto.UsuarioID, DbType.Int32);
            parameters.Add("@Descrip", presupuesto.Descrip, DbType.String);
            parameters.Add("@PrEjecTotal", presupuesto.PrEjecTotal, DbType.Decimal);
            parameters.Add("@PrEjecTotal1", presupuesto.PrEjecTotal1, DbType.Decimal);
            parameters.Add("@EjecMoneda", presupuesto.EjecMoneda, DbType.AnsiStringFixedLength, size: 1);
            parameters.Add("@EjecMoneda1", presupuesto.EjecMoneda1, DbType.AnsiStringFixedLength, size: 1);
            parameters.Add("@Superficie", presupuesto.Superficie, DbType.Decimal);
            parameters.Add("@MesBase", presupuesto.MesBase, DbType.Date);
            parameters.Add("@FechaC", presupuesto.FechaC, DbType.Date);
            parameters.Add("@FechaM", presupuesto.FechaM, DbType.Date);
            parameters.Add("@EsModelo", presupuesto.EsModelo, DbType.Boolean);
            parameters.Add("@TipoCambioD", presupuesto.TipoCambioD, DbType.Decimal);
            parameters.Add("@EgresosTotales", presupuesto.EgresosTotales, DbType.Decimal);
            parameters.Add("@EgresosTotales1", presupuesto.EgresosTotales1, DbType.Decimal);
            parameters.Add("@IngresosTotales", presupuesto.IngresosTotales, DbType.Decimal);
            parameters.Add("@IngresosTotales1", presupuesto.IngresosTotales1, DbType.Decimal);
            parameters.Add("@Conceptos", tableConceptos.AsTableValuedParameter("dbo.TT_Conceptos"));
            parameters.Add("@Relaciones", tableRelaciones.AsTableValuedParameter("dbo.TT_Relaciones"));

            try
                {
                // El procedimiento devuelve el ID del presupuesto afectado
                var result = await db.QueryFirstOrDefaultAsync<int>(
                    "ProcesarPresupuesto",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
                }
            catch (SqlException ex)
                {
                throw new Exception($"Error al procesar el presupuesto: {ex.Message}", ex);
                }
            }


        public async Task<(List<ConceptoDTO> Conceptos, List<RelacionDTO> Relaciones)> ObtenerConceptosYRelacionesAsync(int presupuestoID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@PresupuestoID", presupuestoID, DbType.Int32);

                try
                    {
                    using (var multi = await db.QueryMultipleAsync(
                        "ObtenerConceptosYRelaciones",
                        parameters,
                        commandType: CommandType.StoredProcedure))
                        {
                        var conceptos = (await multi.ReadAsync<ConceptoDTO>()).ToList();
                        var relaciones = (await multi.ReadAsync<RelacionDTO>()).ToList();

                        return (conceptos, relaciones);
                        }
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al obtener conceptos y relaciones del presupuesto {presupuestoID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task BorrarPresupuestoAsync(int presupuestoID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", presupuestoID, DbType.Int32);

                try
                    {
                    await db.ExecuteAsync(
                        "BorrarPresupuesto",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al borrar el presupuesto con ID {presupuestoID}: {ex.Message}", ex);
                    }
                }
            }
      
        }
    }





