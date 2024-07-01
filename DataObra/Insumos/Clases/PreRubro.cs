using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class PreRubro
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        public int EditadoID { get; set; } // Ultima edición
        public DateTime Editado { get; set; }
        public int ObraID { get; set; }
        public int PresupuestoID { get; set; }
        #endregion
        #region DATOS
        public string Descrip { get; set; }
        public bool Tipo { get; set; } // Rubro o SubRubro?
        public string Orden { get; set; }  // 1.1 1.2  1.21 etc. 
        #endregion
        #region VALORES
        public decimal CostoPesos { get; set; }
        public decimal CostoDolares { get; set; }
        public decimal VentaPesos { get; set; }
        public decimal VentaDolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelTar { get; set; } // Si tiene tareas relacionadas
        #endregion
    }
}
