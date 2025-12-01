using Bibioteca.Clases;
using Biblioteca.DTO;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Servidor.Repositorios
    {
    public class rControl
        {
        private readonly string _connectionString;

        public rControl(string connectionString)
            {
            _connectionString = connectionString;
            }

        // Resultado compuesto: Conceptos, Relaciones y Detalles (DocumentosDet) para un presupuesto
        public class ConceptosRelacionesYDetallesResult
            {
            public List<ConceptoDTO> Conceptos { get; set; } = new();
            public List<RelacionDTO> Relaciones { get; set; } = new();
            public List<GastoDetalleDTO> Detalles { get; set; } = new();
            }

        // Nuevo método: consume dbo.ObtenerConceptosRelacionesYDocumentosDet y usa las DTO existentes
        public async Task<ConceptosRelacionesYDetallesResult> ObtenerConceptosRelacionesYDetallesAsync(int presupuestoID)
            {
            using var db = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@PresupuestoID", presupuestoID, DbType.Int32);

            try
                {
                using var multi = await db.QueryMultipleAsync(
                    "ObtenerConceptosRelacionesYDocumentosDet",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // 1) Conceptos
                var conceptos = (await multi.ReadAsync<ConceptoDTO>()).ToList();

                // 2) Relaciones
                var relaciones = (await multi.ReadAsync<RelacionDTO>()).ToList();

                // 3) DocumentosDet -> mapear a GastoDetalleDTO (sin Importe calculado desde SQL)
                var detalles = (await multi.ReadAsync<GastoDetalleDTO>()).ToList();

                // Nota: Importe se calculará en la aplicación; si quieres asegurar coherencia aquí, puedes:
                // foreach (var d in detalles) d.Importe = Math.Round(d.Cantidad * d.PrecioUnitario, 2, MidpointRounding.AwayFromZero);

                return new ConceptosRelacionesYDetallesResult
                    {
                    Conceptos = conceptos,
                    Relaciones = relaciones,
                    Detalles = detalles
                    };
                }
            catch (SqlException ex)
                {
                throw new Exception($"Error al obtener conceptos, relaciones y detalles del presupuesto {presupuestoID}: {ex.Message}", ex);
                }
            }
        }
    }