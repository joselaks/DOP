using System;

namespace Biblioteca.DTO
{
    public class AgrupadorDTO
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public char TipoID { get; set; }
        public string Tipo { get; set; }
        public DateTime Editado { get; set; } // Cambiado a DateTime para coincidir con la tabla
        public string Descrip { get; set; }
        public string? Numero { get; set; }
        public bool Active { get; set; }
    }
}

