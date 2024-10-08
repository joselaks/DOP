using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sur
{
    public class SurCuenta
    {
        #region SISTEMA
        public short ID { get; set; } // Smallint
        public DateTime Creada { get; set; } // Date
        public DateTime? UltimoIngreso { get; set; } // DateTime
        #endregion

        #region USUARIO
        public string Alias { get; set; } = string.Empty; // varchar(12) No puede ser null
        public byte Usuarios { get; set; } // TinyInt Cantidad maxima activos (sin limite vinculados)
        public bool Active { get; set; }
        public bool Demo { get; set; }
        public bool Otra { get; set; }
        public bool Paga { get; set; }
        public DateTime? Vence { get; set; } // Date Si es paga, cuando vence
        #endregion

        #region PROPIETARIO
        public int PropietarioCuentaID { get; set; } // Tabla de Usuarios
        public string Descrip { get; set; } = string.Empty; // Nombre del usuario o empresa propietaria
        public string? Notas { get; set; } // Uso interno
        #endregion
    }

    class SurCuentaRel
    {
        public short CuentaID { get; set; } // Superior
        public int UsuarioID { get; set; } // Inferior
        public string Rol { get; set; } = string.Empty; // Falta definir
        public bool Active { get; set; }  // Si el usuario ya no tiene acceso a la cuenta
        public string? Notas { get; set; }  // Del Admin que genera el vinculo Cuenta Usuario
    }
}
