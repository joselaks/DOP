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
        public int ID { get; set; } // Int 
        public short CuentaID { get; set; } // Smallint
        public int UsuarioID { get; set; } // Creador
        public short ListaID { get; set; } // Smallint
        public char TipoID { get; set; } // Material Equipo Mano de Obra, Otros etc.
        #endregion
        #region USUARIO / PROVEEDOR
        public string Descrip { get; set; } = string.Empty; // Del proveedor
        public string Unidad { get; set; } = string.Empty; // Del proveedor
        public string Codigo { get; set; } = string.Empty; // Del proveedor
        public DateTime Fecha { get; set; }  // De validez del precio
        public decimal Precio { get; set; } // Decimal 19.2
        public bool Pesos { get; set; }  // Si es false, usar factor precio al vincular con Insumo en pesos
        #endregion
    }

    class ArticuloRel
    {
        public int ArticuloID { get; set; } // Int
        public int MaestroInsumoID { get; set; }  // Int
        public short CuentaID { get; set; } // Smallint
        public decimal FactorPrecio { get; set; } // Decimal 9,2
        public decimal FactorUnidad { get; set; } // Decimal 9,2
        public bool Selecccionado { get; set; } // bit Si es Articulo por defecto para ese insumos y cuenta
    }
    public class ArticuloListas
    {
        #region SISTEMA
        public short ID { get; set; } // Smallint
        public short CuentaID { get; set; } // Smallint
        public int UsuarioID { get; set; } // Int Creador
        public char TipoID { get; set; }  // Materiales, Equipos, Varios, etc. 
        #endregion
        #region USUARIO
        public int? EntidadID { get; set; } // Agrupador Entidad si corresponde
        public string Descrip { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }  // De validez de los precios según Usuario
        public char Moneda { get; set; } // P Pesos default D Dolares E Euro R Reales
        public bool Active { get; set; } // False para que no aparezca como opción por Usuario/Cuenta
        #endregion
    }
}
