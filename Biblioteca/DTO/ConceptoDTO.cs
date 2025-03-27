using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class ConceptoDTO
    {
        public int PresupuestoID { get; set; }
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public char Tipo { get; set; }
        public decimal? Precio1 { get; set; }
        public decimal? Precio2 { get; set; }
        public DateTime? FechaPrecio { get; set; }
        public string Unidad { get; set; }
        public decimal? CanPr { get; set; }
        public decimal? CanPe { get; set; }
        public decimal? CanCo { get; set; }
        public decimal? CanEn { get; set; }
        public decimal? CanFa { get; set; }
        public decimal? CanEj { get; set; }
        public decimal? UltimoPrecio1 { get; set; }
        public decimal? UltimoPrecio2 { get; set; }
        public DateTime? FechaUltimoPrecio { get; set; }
        public int? DocumentoID { get; set; }
        public int? InsumoID { get; set; }

        public char Accion { get; set; }
    }
}
