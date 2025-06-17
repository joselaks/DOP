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
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para Conceptos
                var tableConceptos = new DataTable();
                tableConceptos.Columns.Add("PresupuestoID", typeof(int));
                tableConceptos.Columns.Add("ConceptoID", typeof(string));
                tableConceptos.Columns.Add("Descrip", typeof(string));
                tableConceptos.Columns.Add("Tipo", typeof(char));
                tableConceptos.Columns.Add("Unidad", typeof(string));
                tableConceptos.Columns.Add("PrEjec", typeof(decimal));
                tableConceptos.Columns.Add("PrVent", typeof(decimal));
                tableConceptos.Columns.Add("EjecMoneda", typeof(char));
                tableConceptos.Columns.Add("VentMoneda", typeof(char));
                tableConceptos.Columns.Add("MesBase", typeof(DateTime));
                tableConceptos.Columns.Add("CanTotalEjec", typeof(decimal));
                tableConceptos.Columns.Add("InsumoID", typeof(int));
                tableConceptos.Columns.Add("Accion", typeof(char));

                foreach (var c in conceptos)
                    {
                    tableConceptos.Rows.Add(
                        c.PresupuestoID,
                        c.ConceptoID,
                        c.Descrip,
                        c.Tipo,
                        c.Unidad,
                        c.PrEjec,
                        c.PrVent,
                        c.EjecMoneda,
                        c.VentMoneda,
                        c.MesBase,
                        c.CanTotalEjec,
                        c.InsumoID.HasValue ? (object)c.InsumoID.Value : DBNull.Value,
                        c.Accion.HasValue ? (object)c.Accion.Value : DBNull.Value
                    );
                    }

                // Crear DataTable para Relaciones
                var tableRelaciones = new DataTable();
                tableRelaciones.Columns.Add("PresupuestoID", typeof(int));
                tableRelaciones.Columns.Add("CodSup", typeof(string));
                tableRelaciones.Columns.Add("CodInf", typeof(string));
                tableRelaciones.Columns.Add("CanEjec", typeof(decimal));
                tableRelaciones.Columns.Add("CanVenta", typeof(decimal));
                tableRelaciones.Columns.Add("OrdenInt", typeof(short));
                tableRelaciones.Columns.Add("Accion", typeof(char));

                foreach (var r in relaciones)
                    {
                    tableRelaciones.Rows.Add(
                        r.PresupuestoID,
                        r.CodSup,
                        r.CodInf,
                        r.CanEjec,
                        r.CanVenta,
                        r.OrdenInt,
                        r.Accion.HasValue ? (object)r.Accion.Value : DBNull.Value
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@ID", presupuesto.ID == 0 ? (object)DBNull.Value : presupuesto.ID, DbType.Int32);
                parameters.Add("@CuentaID", presupuesto.CuentaID, DbType.Int32);
                parameters.Add("@UsuarioID", presupuesto.UsuarioID, DbType.Int32);
                parameters.Add("@Descrip", presupuesto.Descrip, DbType.String);
                parameters.Add("@PrEjecTotal", presupuesto.PrEjecTotal, DbType.Decimal);
                parameters.Add("@PrEjecDirecto", presupuesto.PrEjecDirecto, DbType.Decimal);
                parameters.Add("@EjecMoneda", presupuesto.EjecMoneda, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@PrVentaTotal", presupuesto.PrVentaTotal, DbType.Decimal);
                parameters.Add("@PrVentaDirecto", presupuesto.PrVentaDirecto, DbType.Decimal);
                parameters.Add("@VentaMoneda", presupuesto.VentaMoneda, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@Superficie", presupuesto.Superficie, DbType.Decimal);
                parameters.Add("@MesBase", presupuesto.MesBase, DbType.Date);
                parameters.Add("@FechaC", presupuesto.FechaC, DbType.Date);
                parameters.Add("@FechaM", presupuesto.FechaM, DbType.Date);
                parameters.Add("@EsModelo", presupuesto.EsModelo, DbType.Boolean);
                parameters.Add("@TipoCambioD", presupuesto.TipoCambioD, DbType.Decimal);
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



