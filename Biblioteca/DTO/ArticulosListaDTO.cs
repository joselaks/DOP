using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ArticulosListaDTO
        {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public string TipoID { get; set; }
        public int? EntidadID { get; set; }
        public string Descrip { get; set; }
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; }
        public bool Active { get; set; }
        }
    }
