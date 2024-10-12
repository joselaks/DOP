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
        public int ID { get; set; }
        public short CuentaID { get; set; } // Smallint
        public int UsuarioID { get; set; } // Ultimo en modificar
        public DateTime Editado { get; set; } // Ultima fecha de modificacion
        public int TipoID { get; set; } // De la tabla clasificadores
        #endregion
        #region AGRUPADORES
        public int? TesoreriaID { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? EntidadID { get; set; }
        public char? EntidadTipo { get; set; } // Cliente Proveedor Contratista Obrero oTro
        #endregion
        #region DOCUMENTOS
        public int? CompraID { get; set; } // Impuesto detalle
        public int? ContratoID { get; set; } // Impuesto detalle
        public int? FacturaID { get; set; } // Impuesto detalle
        public int? OrdenID { get; set; } // Impuesto detalle
        public int? CobroID { get; set; } // Impuesto detalle
        public int? PagoID { get; set; } // Impuesto detalle
        #endregion
        #region DATOS
        public DateTime Fecha { get; set; } // Date
        public int? Comprobante { get; set; } // Relacionado con el impuestop, del usuario
        public string Descrip { get; set; } = string.Empty; // Detalle del impuesto
        public string? Notas { get; set; } // Del usuario
        public bool Previsto { get; set; } // En Compras o Contratos son impuestos no confirmados
        #endregion
        #region TOTALES
        public decimal SumaPesos { get; set; } // Facturas de Venta, Retenciones realizadas
        public decimal RestaPesos { get; set; } // Facturas de Compra, Percepcioens recibidas
        public decimal Alicuota { get; set; } // Porcentual aplicado decimal 9,2
        #endregion
        #region RELACIONES
        public int? MovimientoID { get; set; } // Si el impuesto esta asociado a un movimiento, ejemplo pagar el saldo mensual de IVA
        #endregion
    }
}
