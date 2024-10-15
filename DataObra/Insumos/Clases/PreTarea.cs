using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class PreTarea  // Tarea en un Presupuesto
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int ObraID { get; set; } // Creador
        public int PresupestoID { get; set; } // Creador
        public int UsuarioID { get; set; } // Creador
        public int EditadoID { get; set; } // Ultima edición
        public DateTime Editado { get; set; }
        //public int TipoID { get; set; } //
        #endregion
        #region DATOS
        public string Unidad { get; set; }
        public string Descrip { get; set; }
        public string Memoria { get; set; } // Memoria descriptiva de la Tarea
        public string Codigo { get; set; }
        public string Orden { get; set; }  // 1.1 1.2  1.21 etc. ?
        #endregion
        #region VALORES Unitarios
        public decimal CostoPesos { get; set; }
        public decimal CostoDolares { get; set; }
        public decimal VentaPesos { get; set; }
        public decimal VentaDolares { get; set; }
        #endregion
        #region CANTIDADES
        public decimal CantPrevista { get; set; }
        public decimal CantCertificada { get; set; }
        #endregion
        #region RELACIONES
        public int? MaestroTareaID { get; set; }  // Maestro del Usuario
        public int? SysMaestroTareaID { get; set; }  // Maestro DO
        public bool RelIns { get; set; } // Si tiene insumos o es global
        public bool RelAux { get; set; } // Si es auxiliar
        #endregion
    }
    class PreRelacion  // Relacion de Tarea con Insumos del Presupuesto
    {
        public int TareaID { get; set; }
        public int InsumoID { get; set; }
        public short CuentaID { get; set; }
        public decimal Cantidad { get; set; }
    }
}
