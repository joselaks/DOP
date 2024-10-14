using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    public class PreInsumo
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int? ObraID { get; set; } // Si esta incluido en una Obra
        public int PresupuestoID { get; set; } // A cual presupuesto pertenece
        public int UsuarioID { get; set; } // Cambios realizados por usuario en el presupuesto
        public DateTime Editado { get; set; } // DateTime
        #endregion
        #region DATOS INSUMO
        public int? InsumoMaestroID { get; set; } // Si esta vinculado con Maestro o solo Presup.
        public bool? InsumoDO { get; set; } // Vinculo MaestroDO o MaestroUsuario
        // Si es de un Maestro, los datos del insumo no se pueden cambiar salvo el precio.
        public string TipoID { get; set; } // Del presupuesto o Copia del Maestro
        public string Descrip { get; set; } // Del presupuesto o Copia del Maestro
        public string Unidad { get; set; } // Del presupuesto o Copia del Maestro
        public string Codigo { get; set; } // Del presupuesto o Copia del Maestro
        #endregion
        #region DATOS ARTICULO
        public int? ArticuloID { get; set; }  // Si se especifico un Articulo
        public bool ArticuloDO { get; set; } // Si el Articulo es del MaestroDO o del Usuario
        public string ArticuloDescrip { get; set; } // Copia descripcion del articulo
        public string ArticuloProveedor { get; set; } // Copia nombre proveedor del articulo
        public string ArticuloUnidad { get; set; } // Copia unidad del articulo
        public decimal ArticuloFactor { get; set; } // Decimal 9,2
        public DateTime ArticuloFecha { get; set; } // Date
        #endregion
        #region VALORES // Unitarios
        public decimal CostoPesos { get; set; } 
        public decimal CostoDolares { get; set; }
        public decimal VentaPesos { get; set; }
        public decimal VentaDolares { get; set; }
        #endregion
        #region CANTIDADES
        public decimal CantPrevista { get; set; } // En todo el presupuesto
        public decimal? CantPedida { get; set; } // Actualiza con cada Pedido
        public decimal? CantComprada { get; set; } // Actualiza con cada Compra
        public decimal? CantEntregada { get; set; } // Actualiza con cada Remito
        public decimal? CantFacturada { get; set; } // Actualiza con cada Factura
        public decimal? CantPartes { get; set; } // Actualiza con cada Parte
        #endregion
    }
}
