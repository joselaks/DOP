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
        public string ConceptoID { get; set; } // Corresponde a ConceptoID en SQL
        public string Descrip { get; set; }
        public char Tipo { get; set; }
        public string Unidad { get; set; }
        public decimal PrEjec { get; set; }
        public decimal PrVent { get; set; }
        public char EjecMoneda { get; set; }
        public char VentMoneda { get; set; }
        public DateTime MesBase { get; set; }
        public decimal CanTotalEjec { get; set; }
        public int? InsumoID { get; set; }
        public char? Accion { get; set; } // Solo necesario si usas el procedimiento de alta/modificación/borrado
        }

    }
