using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class MaestroTarea
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public int EditadoID { get; set; }
        public DateTime Editado { get; set; }
        #endregion
        #region DATOS
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public string Codigo { get; set; }
        public string Memoria { get; set; } // Memoria descriptiva de la Tarea
        public string Etiqueta { get; set; } // Clasificacción opcional del usuario
        public bool Auxiliar { get; set; } // Si actua como insumo auxiliar
        #endregion
        #region VALORES
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelIns { get; set; } // Si tiene insumos (o es global...)
        #endregion
    }
    class MaestroRelacion  // Relacion de Tarea con Insumos
    {
        public int TareaID { get; set; }
        public int InsumoID { get; set; }
        public int CuentaID { get; set; }
        public string TipoRel { get; set; }
        public decimal Cantidad { get; set; }
    }
}
