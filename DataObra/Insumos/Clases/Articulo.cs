using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class Articulo
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        public int TipoID { get; set; }
        public int ListaID { get; set; }
        #endregion
        #region DATOS
        public string Descrip { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }  // De validez del precio
        #endregion
        #region VALORES
        public decimal Precio { get; set; }
        public bool Pesos { get; set; }  // Si el precio es en dolares, usar factor precio al vincular con Insumo en pesos
        #endregion
    }

    class ArticuloRel
    {
        public int ArticulosID { get; set; }
        public int MaestroInsumoID { get; set; }
        public int PrecioNro { get; set; }
        public decimal FactorPrecio { get; set; }
        public decimal FactorUnidad { get; set; }
        public bool Selecccionado { get; set; }
    }
    public class ArticuloListas
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        public string TipoID { get; set; } = string.Empty;  // Materiales, Equipos, etc.
        #endregion
        #region DATOS
        public int? EntidadID { get; set; } // Agrupador Entidad si corresponde
        public string Descrip { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }  // De validez de los precios
        public bool Pesos { get; set; }
        #endregion
    }
}
