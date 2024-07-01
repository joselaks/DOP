using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sistema.Clases
{
    class SysClasificador
    {
        public int ID { get; set; }
        public string Descrip { get; set; } = string.Empty;
        public bool Active { get; set; }
        public char Tipo { get; set; }
        public int? Numero { get; set; }
    }
}
