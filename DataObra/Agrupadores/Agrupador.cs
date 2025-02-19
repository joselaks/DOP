using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Agrupadores
{
    public class Agrupador
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; } // Smallint en SQL
        public int UsuarioID { get; set; } // Int 
        public char TipoID { get; set; } // Tinyint o Char si es mas eficiente
        public DateTime Editado { get; set; } // Date del sistema
        #endregion
        #region USUARIO
        public string Descrip { get; set; } = string.Empty; // varchar(50)
        public string? Numero { get; set; } // Dato opcional ingresado x usuario - null
        public bool Active { get; set; } // Si aparece en desplegables, defindo x usuario x cuenta
        #endregion
    }

    public class AgrupadorRel
    {
        public int SuperiorID { get; set; } // int Agrupador
        public int InferiorID { get; set; } // int Agrupador
        public short CuentaID { get; set; } // Smallint Dato para facilitar borrado de cuenta y verificar integridad
        public bool Active { get; set; } // bit
        
        //public string? Tipo { get; set; } // Definir
    }
}
