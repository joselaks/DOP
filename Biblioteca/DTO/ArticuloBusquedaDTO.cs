using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ArticuloBusquedaDTO
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
        public string ListaDescrip { get; set; }
        }

    public class ArticuloInsumoDTO
        {
        public int ArticuloID { get; set; }
        public int InsumoID { get; set; }
        public string Descrip { get; set; }
        public string TipoID { get; set; }
        public string Unidad { get; set; }
        public decimal FactorPrecio { get; set; }
        public decimal FactorUnidad { get; set; }
        public bool Seleccionado { get; set; }  
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; }
        public string Nota { get; set; }
        public string URL { get; set; } // Nueva propiedad  
        public string ListaDescrip { get; set; }
        }



    }
