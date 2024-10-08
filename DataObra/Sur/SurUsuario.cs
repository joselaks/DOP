using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sur
{
    public class SurUsuario
    {
        #region SISTEMA
        public int ID { get; set; }
        public DateTime Fecha { get; set; }  // Cuando se dio de alta el usuario
        public int? SesionID { get; set; } // Si hay nro esta abierta, se borra al salir
        public DateTime? SesionUltima { get; set; }  // Ultimo ingreso, sesion abierta o cerrada
        public string? Notas { get; set; } // Interno de Sur
        #endregion
        #region LOGIN
        public string Email { get; set; } = string.Empty;
        public string Pass { get; set; }
        public bool Active { get; set; }
        #endregion
        #region USUARIO
        public int? DNI { get; set; }
        public DateTime Nacimiento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Ocupacion { get; set; }  // Profesion, etc.
        public string Ciudad { get; set; } = string.Empty; // Ciudad o Localidad
        public string Estado { get; set; } = string.Empty; // Provincia
        public string Pais { get; set; } = string.Empty;
        public string? Celular { get; set; }
        public string? Email2 { get; set; }
        #endregion
    }

    public class SurUsuarioLog
    {
        #region SISTEMA
        public int ID { get; set; }  // Sesion ID
        public DateTime Entrada { get; set; } // Fecha y Hora
        public DateTime Salida { get; set; } // Fecha y Hora
        public int UsuarioID { get; set; }
        public string Usuario { get; set; } = string.Empty;  // Mostrar Nombre de Usuario
        public string Alias { get; set; } = string.Empty; // de la Cuenta
        #endregion
    }
}
