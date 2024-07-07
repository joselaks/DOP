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
        public int CuentaID { get; set; }  // Opcional
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public int TipoID { get; set; }
        #endregion
        #region DATOS
        public string Descrip { get; set; } = string.Empty;
        public bool Active { get; set; }
        public int Numero { get; set; }
        #endregion
    }

    public class AgrupadorRel
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public int CuentaID { get; set; }
        public bool Active { get; set; }
        public string? Tipo { get; set; } // Definir
    }
}
