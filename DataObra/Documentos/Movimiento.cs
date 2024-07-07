using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos
{
    public class Movimiento
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
        public int? GastoID { get; set; }
        public int? OrdenID { get; set; }
        public int? CobroID { get; set; }
        public int? PagoID { get; set; }
        #endregion
        #region DATOS
        public DateTime Fecha { get; set; }
        public int Comprobante { get; set; } // Uso bancario
        public string Descrip { get; set; } = string.Empty;
        public int Numero1 { get; set; }
        public int Numero2 { get; set; }
        public int Numero3 { get; set; }
        public string? Notas { get; set; }
        public DateTime ConciliadoFecha { get; set; }
        public int ConciliadoUsuario { get; set; }
        public bool ChequeProcesado { get; set; }
        public bool Previsto { get; set; }
        public bool Desdoblado { get; set; }
        #endregion
        #region TOTALES
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal SumaDolares { get; set; }
        public decimal RestaDolares { get; set; }
        public decimal Cambio { get; set; } // Va o no?
        #endregion
        #region RELACIONES
        public bool RelMov { get; set; } // Entre movimientos
        public int? ImpuestoID { get; set; } // Impuestos asociado al movimiento, ejemplo percepción bancaria de IVA
        #endregion
    }

    class MovimientoRel
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public int CuentaID { get; set; }
    }
}
