using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class RelacionDTO
        {
        public int PresupuestoID { get; set; }
        public string CodSup { get; set; }
        public string CodInf { get; set; }
        public decimal CanEjec { get; set; }
        public decimal CanVenta { get; set; }
        public short OrdenInt { get; set; }
        public char? Accion { get; set; } // Solo necesario si usas el procedimiento de alta/modificación/borrado
        }

    }
