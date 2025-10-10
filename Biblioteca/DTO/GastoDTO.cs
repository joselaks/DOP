using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class GastoDTO
        {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public byte TipoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime CreadoFecha { get; set; } 
        public DateTime? EditadoFecha { get; set; }
        public int? PresupuestoID { get; set; }
        public string? Entidad { get; set; }
        public string? Documento { get; set; }
        public string? Descrip { get; set; } 
        public string? Notas { get; set; } 
        public decimal Importe { get; set; }
        public char Moneda { get; set; }
        }
    }

