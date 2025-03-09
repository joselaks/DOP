using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class UsuarioDTO
    {
        public int ID { get; set; }
        public DateTime? Fecha { get; set; }
        public int? SesionID { get; set; }
        public DateTime? SesionUltima { get; set; }
        public string Notas { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool Active { get; set; }
        public int? DNI { get; set; }
        public DateTime? Nacimiento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Ocupacion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Celular { get; set; }
        public string Email2 { get; set; }
    }

    public class CredencialesUsuarioDTO
    {
        public string? Token { get; set; }
        //public DateTime Expiracion { get; set; }
        public UsuarioDTO? DatosUsuario { get; set; }

    }
}
