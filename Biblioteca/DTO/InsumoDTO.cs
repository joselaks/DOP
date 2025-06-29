using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class InsumoDTO
        {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public string Tipo { get; set; }
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public string Moneda { get; set; }
        public decimal Precio { get; set; }
        public string Codigo { get; set; }
        public bool ArticulosRel { get; set; }
        }
    }
