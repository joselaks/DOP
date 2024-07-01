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
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int ObraID { get; set; }
        public int PresupuestoID { get; set; }
        public int UsuarioID { get; set; } // Queda registro de cambios por usuario en el presupuesto
        public DateTime Editado { get; set; }
        #endregion
        #region DATOS INSUMO
        public int InsumoMaestroID { get; set; }
        public bool InsumoDO { get; set; } // Vinculo con cual Maestro
        public string TipoID { get; set; }
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public string Codigo { get; set; }
        #endregion
        #region DATOS ARTICULO
        public int ArticuloID { get; set; }  // Si se especifico un Articulo
        public bool ArticuloDO { get; set; } // Si el Articulo es del MaestroDO o del Usuario
        public string ArticuloDescrip { get; set; }
        public string ArticuloProveedor { get; set; }
        public string ArticuloUnidad { get; set; }
        public decimal ArticuloFactor { get; set; }
        public DateTime ArticuloFecha { get; set; }
        #endregion
        #region VALORES
        public decimal CostoPesos { get; set; }
        public decimal CostoDolares { get; set; }
        public decimal VentaPesos { get; set; }
        public decimal VentaDolares { get; set; }
        #endregion
        #region CANTIDADES
        public decimal CantPrevista { get; set; } // En todo el presupuesto
        public decimal CantPedida { get; set; }
        public decimal CantComprada { get; set; }
        public decimal CantEntregada { get; set; }
        public decimal CantFacturada { get; set; }
        public decimal CantPartes { get; set; }
        #endregion
    }
}
