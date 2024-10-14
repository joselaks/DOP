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
        public int ID { get; set; } // ID único del movimiento
        public short CuentaID { get; set; } // ID de la cuenta
        public int UsuarioID { get; set; } // ID del usuario que creó/modificó el movimiento
        public DateTime Editado { get; set; } // Fecha y hora de la última edición
        public int TipoID { get; set; } // De la tabla Clasificadores, tipo de movimiento
        public string Descrip { get; set; } = string.Empty; // De la tabla Clasificadores
        #endregion

        #region AGRUPADORES
        public int? TesoreriaID { get; set; } // ID de la tesorería
        public int? AdminID { get; set; } // ID de la administración
        public int? ObraID { get; set; } // ID de la obra
        public int? EntidadID { get; set; } // ID de la entidad
        public int? EntidadTipo { get; set; } // Tipo de entidad
        #endregion

        #region DOCUMENTOS
        public int? CompraID { get; set; } // ID de la compra
        public int? ContratoID { get; set; } // ID del contrato
        public int? FacturaID { get; set; } // ID de la factura
        public int? GastoID { get; set; } // ID del gasto
        public int? OrdenID { get; set; } // ID de la orden
        public int? CobroID { get; set; } // ID del cobro
        public int? PagoID { get; set; } // ID del pago
        #endregion

        #region USUARIO
        public DateTime Fecha { get; set; } // Date Fecha del movimiento
        public int Comprobante { get; set; } // Uso bancario
        
        public int? Numero { get; set; } // Nro. de Cheque o moviemiento segun banco
        public string? Notas { get; set; } // Varchar(50) Notas del usuario
        public DateTime ConciliadoFecha { get; set; } // Date Fecha de conciliación
        public int ConciliadoUsuario { get; set; } // Usuario que concilió
        public bool ChequeProcesado { get; set; } // Si el cheque ha sido procesado
        public bool Previsto { get; set; } // Si el movimiento es previsto, para proyeccuiones
        public bool Desdoblado { get; set; } // Si el movimiento está desdoblado
        #endregion

        #region TOTALES
        public decimal SumaPesos { get; set; } // Suma en pesos
        public decimal RestaPesos { get; set; } // Resta en pesos
        public decimal SumaDolares { get; set; } // Suma en dólares
        public decimal RestaDolares { get; set; } // Resta en dólares
        public decimal Cambio { get; set; } // Tipo de cambio
        #endregion

        #region RELACIONES
        public bool RelMov { get; set; } // Si está relacionado con otros movimientos, ejemplo tansf. de otra cuenta
        public int? ImpuestoID { get; set; } // ID del impuesto asociado
        #endregion
    }

    class MovimientoRel
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public short CuentaID { get; set; }
    }
}
