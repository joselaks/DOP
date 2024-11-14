using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
{
    public class AgrupadorAPI
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public char TipoID { get; set; }
        public DateTime Editado { get; set; }
        public string Descrip { get; set; }
        public int? Numero { get; set; }
        public bool Active { get; set; }

    }
}
