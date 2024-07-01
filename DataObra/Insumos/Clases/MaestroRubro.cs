using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class MaestroRubro
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        #endregion
        #region DATOS
        public string Descrip { get; set; }
        public string Etiqueta { get; set; }
        #endregion
    }
}
