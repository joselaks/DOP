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

                bool esNuevo = !presupuesto.ID.HasValue || presupuesto.ID == 0;

                foreach (var c in conceptos)
                    {
                    tableConceptos.Rows.Add(
                        esNuevo ? (object)DBNull.Value : c.PresupuestoID,
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
                        esNuevo ? (object)DBNull.Value : r.PresupuestoID,
                        r.CodSup,
                        r.CodInf,
                        r.CanEjec,
                        r.CanVenta,
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
        public async Task<(List<ConceptoMDTO> Conceptos, List<RelacionMDTO> Relaciones)> ObtenerConceptosYRelacionesMaestroAsync(int usuarioID)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);

                try
                    {
                    using (var multi = await db.QueryMultipleAsync(
                        "ObtenerConceptosYRelacionesMaestro",
                        parameters,
                        commandType: CommandType.StoredProcedure))
                        {
                        var conceptos = (await multi.ReadAsync<ConceptoMDTO>()).ToList();
                        var relaciones = (await multi.ReadAsync<RelacionMDTO>()).ToList();

                        return (conceptos, relaciones);
                        }
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al obtener conceptos y relaciones maestro del usuario {usuarioID}: {ex.Message}", ex);
                    }
                }
            }

        public async Task<string> ProcesarTareaMaestroAsync(int usuarioID, List<ConceptoMDTO> conceptos, List<RelacionMDTO> relaciones)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Crear DataTable para ConceptosM
                var tableConceptosM = new DataTable();
                tableConceptosM.Columns.Add("UsuarioID", typeof(int));
                tableConceptosM.Columns.Add("ConceptoID", typeof(string));
                tableConceptosM.Columns.Add("Descrip", typeof(string));
                tableConceptosM.Columns.Add("Tipo", typeof(string));
                tableConceptosM.Columns.Add("Unidad", typeof(string));
                tableConceptosM.Columns.Add("PrEjec", typeof(decimal));
                tableConceptosM.Columns.Add("EjecMoneda", typeof(string));
                tableConceptosM.Columns.Add("MesBase", typeof(DateTime));
                tableConceptosM.Columns.Add("InsumoID", typeof(int));
                tableConceptosM.Columns.Add("Accion", typeof(char));

                foreach (var c in conceptos)
                    {
                    tableConceptosM.Rows.Add(
                        c.UsuarioID,
                        c.ConceptoID,
                        c.Descrip,
                        c.Tipo,
                        c.Unidad,
                        c.PrEjec,
                        c.EjecMoneda,
                        c.MesBase,
                        c.InsumoID.HasValue ? (object)c.InsumoID.Value : DBNull.Value,
                        c.Accion
                    );
                    }

                // Crear DataTable para RelacionesM
                var tableRelacionesM = new DataTable();
                tableRelacionesM.Columns.Add("UsuarioID", typeof(int));
                tableRelacionesM.Columns.Add("CodSup", typeof(string));
                tableRelacionesM.Columns.Add("CodInf", typeof(string));
                tableRelacionesM.Columns.Add("CanEjec", typeof(decimal));
                tableRelacionesM.Columns.Add("OrdenInt", typeof(short));
                tableRelacionesM.Columns.Add("Accion", typeof(char));

                foreach (var r in relaciones)
                    {
                    tableRelacionesM.Rows.Add(
                        r.UsuarioID,
                        r.CodSup,
                        r.CodInf,
                        r.CanEjec,
                        r.OrdenInt.HasValue ? (object)r.OrdenInt.Value : DBNull.Value,
                        r.Accion
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuarioID, DbType.Int32);
                parameters.Add("@ConceptosM", tableConceptosM.AsTableValuedParameter("dbo.TT_ConceptosM"));
                parameters.Add("@RelacionesM", tableRelacionesM.AsTableValuedParameter("dbo.TT_RelacionesM"));

                try
                    {
                    var result = await db.QueryFirstOrDefaultAsync<string>(
                        "ProcesarTareaMaestro",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result ?? "OK";
                    }
                catch (SqlException ex)
                    {
                    return $"Error SQL: {ex.Message}";
                    }
                catch (Exception ex)
                    {
                    return $"Error: {ex.Message}";
                    }
                }
            }

        }
    }



