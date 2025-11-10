using System;

namespace Biblioteca.DTO
{
    public class GastoDetalleDTO
    {
        public int ID { get; set; }
        public int GastoID { get; set; }
        public int UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public char TipoID { get; set; }
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
        public string Descrip { get; set; } = string.Empty;    // DB: NOT NULL
        public string Unidad { get; set; } = string.Empty;     // DB: NOT NULL (CHAR(6))
        public decimal Cantidad { get; set; }
        public decimal FactorCantidad { get; set; } = 1.0000m; // Nuevo: coincide con DocumentosDet
        public decimal PrecioUnitario { get; set; }
        public int? ArticuloID { get; set; }
        public string? Articulo { get; set; }
        public string? ListaDePrecios { get; set; }
        public int? MaestroID { get; set; }
        public string? Maestro { get; set; }
        public string? ConceptoMaestroID { get; set; }
        public string? ConceptoMaestro { get; set; }
        public char Moneda { get; set; }
        public decimal Importe { get; set; } = 0;
        public char Accion { get; set; }
    }
}
