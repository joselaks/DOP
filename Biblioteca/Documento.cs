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

    public class InfoDocumento
    {
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
        public char Accion { get; set; }
    }

    public class Movimiento
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public int TipoID { get; set; }
        public string Descrip { get; set; }
        public int? TesoreriaID { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? EntidadID { get; set; }
        public int? EntidadTipo { get; set; }
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? GastoID { get; set; }
        public int? OrdenID { get; set; }
        public int? CobroID { get; set; }
        public int? PagoID { get; set; }
        public DateTime Fecha { get; set; }
        public int Comprobante { get; set; }
        public int? Numero { get; set; }
        public string Notas { get; set; }
        public DateTime ConciliadoFecha { get; set; }
        public int ConciliadoUsuario { get; set; }
        public bool ChequeProcesado { get; set; }
        public bool Previsto { get; set; }
        public bool Desdoblado { get; set; }
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal SumaDolares { get; set; }
        public decimal RestaDolares { get; set; }
        public decimal Cambio { get; set; }
        public bool RelMov { get; set; }
        public int? ImpuestoID { get; set; }
        public char Accion { get; set; }
    }

    public class Impuesto
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; }
        public int TipoID { get; set; }
        public int? TesoreriaID { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? EntidadID { get; set; }
        public char? EntidadTipo { get; set; }
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? OrdenID { get; set; }
        public int? CobroID { get; set; }
        public int? PagoID { get; set; }
        public DateTime Fecha { get; set; }
        public int? Comprobante { get; set; }
        public string Descrip { get; set; }
        public string Notas { get; set; }
        public bool Previsto { get; set; }
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal Alicuota { get; set; }
        public int? MovimientoID { get; set; }
        public char Accion { get; set; }
    }
}


