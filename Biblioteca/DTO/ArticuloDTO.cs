using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ArticuloDTO
        {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public short ListaID { get; set; }
        public int? EntidadID { get; set; }
        public string TipoID { get; set; }
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public decimal UnidadFactor { get; set; }
        public string Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public string Nota { get; set; }
        public string URL { get; set; } // Nueva propiedad
        // Si necesitas soportar operaciones tipo 'A', 'M', 'B' puedes agregar:
        public char? Accion { get; set; }
        }

    public class ArticuloExceDTO
        {
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public decimal Precio { get; set; }
        }


    public class ListaArticuloItem
    {
        public int ID { get; set; }
        public string Descrip { get; set; }
        public string TipoID { get; set; }
        }

    }
