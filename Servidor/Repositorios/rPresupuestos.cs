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

        public async Task<(List<ConceptoDTO> Conceptos, List<RelacionDTO> Relaciones)> ObtenerRegistrosPorPresupuestoIDAsync(int presupuestoID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PresupuestoID", presupuestoID, DbType.Int32);

                try
                {
                    using (var multi = await db.QueryMultipleAsync("ObtenerRegistrosPorPresupuestoID", parameters, commandType: CommandType.StoredProcedure))
                    {
                        var conceptos = (await multi.ReadAsync<ConceptoDTO>()).ToList();
                        var relaciones = (await multi.ReadAsync<RelacionDTO>()).ToList();

                        return (conceptos, relaciones);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Error al obtener los registros por PresupuestoID: {ex.Message}", ex);
                }
            }
        }


        public async Task ProcesarArbolPresupuestoAsync(int presupuestoID, List<ConceptoDTO> listaConceptos, List<RelacionDTO> listaRelaciones)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                var tableConceptos = new DataTable();
                tableConceptos.Columns.Add("PresupuestoID", typeof(int));
                tableConceptos.Columns.Add("Codigo", typeof(string));
                tableConceptos.Columns.Add("Descrip", typeof(string));
                tableConceptos.Columns.Add("Tipo", typeof(char));
                tableConceptos.Columns.Add("Precio1", typeof(decimal));
                tableConceptos.Columns.Add("Precio2", typeof(decimal));
                tableConceptos.Columns.Add("FechaPrecio", typeof(DateTime)); // Cambiar a FechaPrecio
                tableConceptos.Columns.Add("Unidad", typeof(string));
                tableConceptos.Columns.Add("CanPr", typeof(decimal));
                tableConceptos.Columns.Add("CanPe", typeof(decimal));
                tableConceptos.Columns.Add("CanCo", typeof(decimal));
                tableConceptos.Columns.Add("CanEn", typeof(decimal));
                tableConceptos.Columns.Add("CanFa", typeof(decimal));
                tableConceptos.Columns.Add("CanEj", typeof(decimal));
                tableConceptos.Columns.Add("UltimoPrecio1", typeof(decimal));
                tableConceptos.Columns.Add("UltimoPrecio2", typeof(decimal));
                tableConceptos.Columns.Add("FechaUltimoPrecio", typeof(DateTime));
                tableConceptos.Columns.Add("DocumentoID", typeof(int));
                tableConceptos.Columns.Add("InsumoID", typeof(int));
                tableConceptos.Columns.Add("Accion", typeof(char));

                foreach (var item in listaConceptos)
                    {
                    tableConceptos.Rows.Add(
                        presupuestoID, item.Codigo, item.Descrip, item.Tipo, item.Precio1, item.Precio2, item.FechaPrecio, item.Unidad, item.CanPr, item.CanPe, item.CanCo, item.CanEn, item.CanFa, item.CanEj, item.UltimoPrecio1, item.UltimoPrecio2, item.FechaUltimoPrecio, item.DocumentoID, item.InsumoID, item.Accion
                    );
                    }

                var tableRelaciones = new DataTable();
                tableRelaciones.Columns.Add("PresupuestoID", typeof(int));
                tableRelaciones.Columns.Add("OrdenInt", typeof(int));
                tableRelaciones.Columns.Add("Superior", typeof(string));
                tableRelaciones.Columns.Add("Inferior", typeof(string));
                tableRelaciones.Columns.Add("Cantidad", typeof(decimal));
                tableRelaciones.Columns.Add("Accion", typeof(char));

                foreach (var item in listaRelaciones)
                    {
                    tableRelaciones.Rows.Add(
                        presupuestoID, item.OrdenInt, item.Superior, item.Inferior, item.Cantidad, item.Accion
                    );
                    }

                var parameters = new DynamicParameters();
                parameters.Add("@PresupuestoID", presupuestoID, DbType.Int32);
                parameters.Add("@ListaConceptos", tableConceptos.AsTableValuedParameter("dbo.ConceptoType"));
                parameters.Add("@ListaRelaciones", tableRelaciones.AsTableValuedParameter("dbo.RelacionType"));

                try
                    {
                    await db.ExecuteAsync("ProcesaArbolPresupuesto", parameters, commandType: CommandType.StoredProcedure);
                    }
                catch (SqlException ex)
                    {
                    throw new Exception($"Error al procesar el árbol de presupuesto: {ex.Message}", ex);
                    }
                }
            }



        }
    }

