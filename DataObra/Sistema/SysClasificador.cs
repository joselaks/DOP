using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sistema
{
    class Clasificador  // Ciudades, Profesiones, Tipos, Corralones, etc. uso interno
    {
        public int ID { get; set; } // Int
        public char TipoID { get; set; } // C P etc.    
        public string Descrip { get; set; } = string.Empty; // varchar(50)
        public bool Active { get; set; } // bit Si se muestra en los listados
        public short? Numero { get; set; } // Dato opcional
    }
}
