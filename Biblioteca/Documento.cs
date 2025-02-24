using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
{
    public class Documento
    {
        public int? ID { get; set; }
        public short CuentaID { get; set; }
        public byte TipoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime CreadoFecha { get; set; }
        public int EditadoID { get; set; }
        public DateTime EditadoFecha { get; set; }
        public int RevisadoID { get; set; }
        public DateTime RevisadoFecha { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? PresupuestoID { get; set; }
        public int? RubroID { get; set; }
        public int? EntidadID { get; set; }
        public int? DepositoID { get; set; }
        public string Descrip { get; set; }
        public string Concepto1 { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime? Fecha2 { get; set; }
        public DateTime? Fecha3 { get; set; }
        public int Numero1 { get; set; }
        public int Numero2 { get; set; }
        public int Numero3 { get; set; }
        public string Notas { get; set; }
        public bool Active { get; set; }
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        public decimal Impuestos { get; set; }
        public decimal ImpuestosD { get; set; }
        public decimal Materiales { get; set; }
        public decimal ManodeObra { get; set; }
        public decimal Subcontratos { get; set; }
        public decimal Equipos { get; set; }
        public decimal Otros { get; set; }
        public decimal MaterialesD { get; set; }
        public decimal ManodeObraD { get; set; }
        public decimal SubcontratosD { get; set; }
        public decimal EquiposD { get; set; }
        public decimal OtrosD { get; set; }
        public bool RelDoc { get; set; }
        public bool RelArt { get; set; }
        public bool RelMov { get; set; }
        public bool RelImp { get; set; }
        public bool RelRub { get; set; }
        public bool RelTar { get; set; }
        public bool RelIns { get; set; }
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra

        public List<DocumentoDet> DetalleDocumento { get; set; }
        public List<Movimiento> DetalleMovimientos { get; set; }
        public List<Impuesto> DetalleImpuestos { get; set; }
    }

    public class DocumentoRel
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public short CuentaID { get; set; }
        public bool PorInsumos { get; set; }
    }

    public class DocumentoDet
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public char TipoID { get; set; }
        public int? AdminID { get; set; }
        public int? EntidadID { get; set; }
        public int? DepositoID { get; set; }
        public int? AcopioID { get; set; }
        public int? PedidoID { get; set; }
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? RemitoID { get; set; }
        public int? ParteID { get; set; }
        public int? ObraID { get; set; }
        public int? PresupuestoID { get; set; }
        public int? RubroID { get; set; }
        public int? TareaID { get; set; }
        public DateTime? Fecha { get; set; }
        public string ArticuloDescrip { get; set; }
        public decimal ArticuloCantSuma { get; set; }
        public decimal ArticuloCantResta { get; set; }
        public decimal ArticuloPrecio { get; set; }
        public decimal? SumaPesos { get; set; }
        public decimal? RestaPesos { get; set; }
        public decimal? SumaDolares { get; set; }
        public decimal? RestaDolares { get; set; }
        public decimal? Cambio { get; set; }
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra
    }

    public class Impuesto
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
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra
    }

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
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra
    }
}


