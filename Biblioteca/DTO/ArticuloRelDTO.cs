using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ArticuloRelDTO
        {
        public int ArticuloID { get; set; }
        public int InsumoID { get; set; }
        public short CuentaID { get; set; }
        public decimal FactorPrecio { get; set; }
        public decimal FactorUnidad { get; set; }
        public bool Seleccionado { get; set; }
        // Si necesitas soportar operaciones tipo 'A', 'M', 'B' puedes agregar:
        public char? Accion { get; set; }
        }
    }
