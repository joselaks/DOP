using System;

namespace Biblioteca.DTO
{
    public class GastoDetalleDTO
    {
        public int ID { get; set; }
        public int? GastoID { get; set; }
        public int? CobroID { get; set; }                // Coincide con DocumentosDet.CobroID
        public int UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public char TipoID { get; set; } = '0';          // CHAR(1) (se normaliza en el repositorio si es '\0')
        public int? PresupuestoID { get; set; }
        public string? Presupuesto { get; set; }
        public string? RubroID { get; set; }
        public string? Rubro { get; set; }
        public string? TareaID { get; set; }
        public string? Tarea { get; set; }
        public string? AuxiliarID { get; set; }
        public string? Auxiliar { get; set; }
        public string? InsumoID { get; set; }
        public string? Insumo { get; set; }
        public string Descrip { get; set; } = string.Empty;    // DB: NOT NULL (varchar(65))
        public string Unidad { get; set; } = string.Empty;     // DB: NOT NULL (char(6))
        public decimal Cantidad { get; set; }
        public decimal FactorCantidad { get; set; } = 1.0000m; // Coincide con DocumentosDet.FactorConcepto
        public decimal PrecioUnitario { get; set; }
        public int? ArticuloID { get; set; }
        public string? Articulo { get; set; }
        public string? ListaDePrecios { get; set; }
        public int? MaestroID { get; set; }
        public string? Maestro { get; set; }
        public string? ConceptoMaestroID { get; set; }
        public string? ConceptoMaestro { get; set; }
        public char Moneda { get; set; } = 'P';                    // CHAR(1)
        public decimal TipoCambioD { get; set; } = 1.0000000000m;  // Coincide con DocumentosDet.TipoCambioD
        public DateTime? Fecha { get; set; }                       // Coincide con DocumentosDet.Fecha (NULLABLE)
        public decimal Importe { get; set; } = 0m;
        public char Accion { get; set; } = 'A';                    // 'A' = Alta, 'M' = Modificar, 'B' = Borrar (para TVP)
    }
}
