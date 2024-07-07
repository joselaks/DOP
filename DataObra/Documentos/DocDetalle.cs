using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos
{
    public class DocDetalle
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public int TipoID { get; set; } // ??
        #endregion
        #region AGRUPADORES
        public int? AdminID { get; set; }
        public int? EntidadID { get; set; }
        public int? EntidadTipo { get; set; }
        public int? DepositoID { get; set; }
        #endregion
        #region DOCUMENTOS
        public int? AcopioID { get; set; }
        public int? PedidoID { get; set; }
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? RemitoID { get; set; }
        public int? ParteID { get; set; }
        #endregion
        #region IMPUTACION
        public int? ObraID { get; set; }
        public int? PresupuestoID { get; set; }
        public int? RubroID { get; set; }
        public int? TareaID { get; set; }
        #endregion
        #region DATOS ARTICULO
        public DateTime Fecha { get; set; }
        public string ArticuloDescrip { get; set; } = string.Empty;
        public decimal ArticuloCantSuma { get; set; }
        public decimal ArticuloCantResta { get; set; }
        public decimal ArticuloPrecio { get; set; }
        #endregion
        #region VALORES  // Los resta son para nota de crédito
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal SumaDolares { get; set; }
        public decimal RestaDolares { get; set; }
        public decimal Cambio { get; set; } // Va o no?
        #endregion
    }
}
