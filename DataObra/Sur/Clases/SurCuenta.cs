using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sur.Clases
{
    public class SurCuenta
    {
        #region SISTEMA
        public int ID { get; set; } // Null?
        public DateTime Creada { get; set; }
        #endregion
        #region DATOS
        public string Alias { get; set; } = string.Empty; // No puede ser null
        public int Usuarios { get; set; } // Cantidad maxima activos
        public bool Paga { get; set; }
        public DateTime? Vence { get; set; } // Si es paga, cuando vence
        public bool Active { get; set; }
        public bool Demo { get; set; }
        public bool Otra { get; set; }
        #endregion
        public int PropietarioCuentaID { get; set; } // Tabla de Usuarios
        public string Descrip { get; set; } = string.Empty; // Nombre del usuario o empresa propietaria
        public string? Notas { get; set; }
        public DateTime? UltimoIngreso { get; set; }
    }

    class SurCuentaRel
    {
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public string Rol { get; set; } = string.Empty;
        public bool Active { get; set; }  // Si el usuario ya no tiene acceso a la cuenta
        public string? Notas { get; set; }  // Del Admin que genera el vinculo Cuenta Usuario
    }
}
