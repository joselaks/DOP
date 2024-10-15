using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class MaestroRubro // Tabla MaestroRubros
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        #endregion
        #region DATOS
        public string Descrip { get; set; } // varchar 65
        public bool Tipo { get; set; } // Rubro o SubRubro?
        #endregion
    }
}
