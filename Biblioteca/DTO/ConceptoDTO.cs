using System;

namespace Biblioteca.DTO
{
    public class ConceptoDTO
    {
        public int PresupuestoID { get; set; }
        public string ConceptoID { get; set; } // Corresponde a ConceptoID en SQL
        public string Descrip { get; set; }
        public char Tipo { get; set; }
        public string Unidad { get; set; }
        public decimal PrEjec { get; set; }
        public decimal PrEjec1 { get; set; }
        public char EjecMoneda { get; set; }
        public DateTime MesBase { get; set; }
        public decimal CanTotalEjec { get; set; }
        public decimal? CantTotalReal { get; set; }    // nuevo
        public decimal? Existencias { get; set; }      // nuevo
        public decimal? PrReal { get; set; }           // nuevo
        public decimal? PrReal1 { get; set; }          // nuevo
        public int? ArticuloID { get; set; }           // nuevo
        public decimal? FactorArticulo { get; set; }   // nuevo
        public char? Accion { get; set; } // 'A' = Alta, 'M' = Modificar, 'B' = Borrar (para TVP)
    }
}
