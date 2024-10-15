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
        public int ID { get; set; } // Maestro de Tareas del usuario
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        public int EditadoID { get; set; } // Ultima edicion, igual creador al inicio
        public DateTime Editado { get; set; } // date
        #endregion
        #region DATOS
        public string Descrip { get; set; } // varchar 65
        public string Unidad { get; set; } // char 2
        public string Codigo { get; set; } // varchar 20
        public string Memoria { get; set; } // varchar 250 Memoria descriptiva de la Tarea
        public string Etiqueta { get; set; } // Clasificacción opcional del usuario
        public bool Auxiliar { get; set; } // Si actua como insumo auxiliar
        public int RubroGenericoID { get; set; } // Rubro del Maestro
        public int RubroID { get; set; } // Rubro del Maestro
        #endregion
        #region VALORES
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelIns { get; set; } // Si tiene insumos (o es global...)
        #endregion
    }
    class MaestroRel  // Relacion de Tarea con Insumos
    {
        public int TareaID { get; set; }
        public int InsumoID { get; set; }
        public short CuentaID { get; set; }
        public decimal Cantidad { get; set; } // decimal 19,2
    }
}
