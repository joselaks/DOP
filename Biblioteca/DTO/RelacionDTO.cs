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
        public string Superior { get; set; }
        public string Inferior { get; set; }
        public decimal Cantidad { get; set; }
        public int? OrdenInt { get; set; }
        public char Accion { get; set; }
    }
}
