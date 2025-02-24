using Bibioteca.Clases;
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

        public async Task ProcesarArbolPresupuestoAsync(int presupuestoID, List<Concepto> listaConceptos, List<Relacion> listaRelaciones)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var tableConceptos = new DataTable();
                tableConceptos.Columns.Add("Codigo", typeof(string));
                tableConceptos.Columns.Add("Descrip", typeof(string));
                tableConceptos.Columns.Add("Precio", typeof(decimal));
                tableConceptos.Columns.Add("Fecha", typeof(DateTime));
                tableConceptos.Columns.Add("Tipo", typeof(string));
                tableConceptos.Columns.Add("Unidad", typeof(string));
                tableConceptos.Columns.Add("Accion", typeof(char));

                foreach (var item in listaConceptos)
                {
                    tableConceptos.Rows.Add(
                        item.Codigo, item.Descrip, item.Precio, item.Fecha, item.Tipo, item.Unidad, item.Accion
                    );
                }

                var tableRelaciones = new DataTable();
                tableRelaciones.Columns.Add("OrdenInt", typeof(int));
                tableRelaciones.Columns.Add("Superior", typeof(string));
                tableRelaciones.Columns.Add("Inferior", typeof(string));
                tableRelaciones.Columns.Add("Descrip", typeof(string));
                tableRelaciones.Columns.Add("Cantidad", typeof(decimal));
                tableRelaciones.Columns.Add("Accion", typeof(char));

                foreach (var item in listaRelaciones)
                {
                    tableRelaciones.Rows.Add(
                        item.OrdenInt, item.Superior, item.Inferior, item.Descrip, item.Cantidad, item.Accion
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

    public class ProcesarArbolPresupuestoRequest
    {
        public int PresupuestoID { get; set; }
        public List<Concepto> ListaConceptos { get; set; }
        public List<Relacion> ListaRelaciones { get; set; }
    }
}

