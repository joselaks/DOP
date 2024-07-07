using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos
{
    class Impuesto
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public int TipoID { get; set; }
        #endregion
        #region AGRUPADORES
        public int? TesoreriaID { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? EntidadID { get; set; }
        public int? EntidadTipo { get; set; }
        #endregion
        #region DOCUMENTOS
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? OrdenID { get; set; }
        public int? CobroID { get; set; }
        public int? PagoID { get; set; }
        #endregion
        #region DATOS
        public DateTime Fecha { get; set; }
        public int Comprobante { get; set; }
        public string Descrip { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public bool Previsto { get; set; }
        #endregion
        #region TOTALES
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal Alicuota { get; set; } // Va en cada concepto de facturas?
        #endregion
        #region RELACIONES
        public int? MovimientoID { get; set; } // Si el impuesto esta asociado a un movimiento, ejemplo pagar el saldo mensual de IVA
        #endregion
    }
}
