using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class SysMaestroInsumo
    {
        #region SISTEMA
        public int ID { get; set; } // Int
        public int UsuarioID { get; set; } // Int
        public DateTime Editado { get; set; } // Date
        #endregion
        #region DATOS
        public char TipoID { get; set; } // char(1) Materiales Mano de Obra, etc.
        public required string Unidad { get; set; }
        public required string Codigo { get; set; }
        public int ZonaID { get; set; } // Zona de los precios
        #endregion
        #region VALORES
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelArtDO { get; set; } // Si tiene relación con articulos del maestro de DO
        #endregion
    }
}
