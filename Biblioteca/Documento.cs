using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Biblioteca
{

    class DocumentoRel  // Relaciones entre Documentos
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public short CuentaID { get; set; }
        public bool PorInsumos { get; set; } = true; // Si la relación esta dada por compartir detalle de insumos
                                                     // o es manual
    }
    public class Documento
    {
        #region SISTEMA
        public int? ID { get; set; }  // Int
        public short CuentaID { get; set; } // SmallInt
        public byte TipoID { get; set; } // TinyInt o 1 Char
        public string? TipoDoc { get; set; } // Tipo en texto para mostrar
        #endregion
        #region USUARIOS
        public int UsuarioID { get; set; } // Int Creador inicial
        public string? Usuario { get; set; } // Mostrar
        public DateTime CreadoFecha { get; set; } // Date Fecha del primer guardado en la tabla
        public int? EditadoID { get; set; } // Int Ultimo usuario en editar
        public string? Editado { get; set; } // Mostrar usuario
        public DateTime? EditadoFecha { get; set; } // Date Ultima modificación
        public int? AutorizadoID { get; set; } // Int Usuario que Autoriza o Verifica
        public string? Autorizado { get; set; } // Mostrar usuario
        public DateTime? AutorizadoFecha { get; set; } // Date Cuando fue Autorizado
        #endregion
        #region AGRUPADORES
        public int? AdminID { get; set; } // Int Administracion o Empresa
        public string? Admin { get; set; } // Mostrar
        public int? ObraID { get; set; }  // Int
        public string? Obra { get; set; }  // Mostrar
        public int? PresupuestoID { get; set; }  // Int
        public string? Presupuesto { get; set; } // Mostrar
        public int? RubroID { get; set; } // Int
        public string? Rubro { get; set; } // Mostrar
        public int? EntidadID { get; set; } // ID
        public string? Entidad { get; set; } // Mostrar
        public string? EntidadTipo { get; set; }  // Mostrar 
        public int? DepositoID { get; set; } // Int
        public string? Deposito { get; set; } // Mostrar
        #endregion
        #region DATOS
        public string? Descrip { get; set; } // varchar150  Titulo o descripción del usuario
        public int? Concepto1 { get; set; } // varchar150
        public DateTime Fecha1 { get; set; } // Date Fecha del documento ingresada por el usuario
        public DateTime? Fecha2 { get; set; } // Date Fecha de vencimiento o entrega o ...
        public DateTime? Fecha3 { get; set; } // Date Fecha contable o ...
        public int Avance { get; set; } // Int Avance de la obra o porcentaje de avance
        public int? Numero1 { get; set; } // Int en las facturas en numero de sucursal
        public int? Numero2 { get; set; } // Int 
        public int? Numero3 { get; set; } // Int numero de archivo u otro
        public string? Notas { get; set; } // Varchar250 Ingresadas por el usuario
        public bool? Active { get; set; } // Bool
        #endregion
        #region TOTALES
        public decimal Precio1 { get; set; } // Decimal 19.2
        public decimal Precio2 { get; set; } // Decimal 19.2
        public decimal Impuestos { get; set; } // Decimal 19.2
        public decimal ImpuestosD { get; set; } // Decimal 19.2
        #endregion
        #region Por Tipo
        public decimal Materiales { get; set; } // Decimal 19.2
        public decimal ManodeObra { get; set; } // Decimal 19.2
        public decimal Subcontratos { get; set; } // Decimal 19.2
        public decimal Equipos { get; set; } // Decimal 19.2
        public decimal Otros { get; set; } // Decimal 19.2
        public decimal MaterialesD { get; set; } // Decimal 19.2
        public decimal ManodeObraD { get; set; } // Decimal 19.2
        public decimal SubcontratosD { get; set; } // Decimal 19.2
        public decimal EquiposD { get; set; } // Decimal 19.2
        public decimal OtrosD { get; set; } // Decimal 19.2
        #endregion
        #region RELACIONES
        public bool RelDoc { get; set; } // Entre documentos
        public bool RelArt { get; set; } // Articulos Detalle
        public bool RelMov { get; set; } // Movimientos de tesorerias, confirmados o previstos
        public bool RelImp { get; set; } // Impuestos (confirmados o previstos?)
        public bool RelRub { get; set; } // Rubros
        public bool RelTar { get; set; } // Tareas
        public bool RelIns { get; set; } // Insumos
        #endregion
        #region COLECCIONES
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra  ¿Para que aqui?
        public List<DocumentoDet> DetalleDocumento { get; set; }
        public List<Movimiento> DetalleMovimientos { get; set; }
        public List<Impuesto> DetalleImpuestos { get; set; }

        #endregion
    }

    public class InfoDocumento
    {
        public List<DocumentoDet> DetalleDocumento { get; set; }
        public List<Movimiento> DetalleMovimientos { get; set; }
        public List<Impuesto> DetalleImpuestos { get; set; }

    }

    public class DocumentoDet  // Tabla DocumentosDet
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; } // date
        public char TipoID { get; set; } // Insumos Dto Acopio Dto Anticipo Det Certificado
        public char Accion { get; set; }
        #endregion

        #region AGRUPADORES
        public int? AdminID { get; set; }
        public int? EntidadID { get; set; }
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
        public string? RubroID { get; set; } // varchar(20)
        public string? TareaID { get; set; } // varchar(20)
        public string? InsumoID { get; set; } // varchar(20)
        #endregion

        #region DATOS ARTICULO
        public DateTime? Fecha { get; set; } // date
        public string ArticuloDescrip { get; set; } = string.Empty; // varchar(65)
        public decimal ArticuloCantSuma { get; set; } = 0; // decimal(19,2), default ((0))
        public decimal ArticuloCantResta { get; set; } = 0; // decimal(19,2), default ((0)), Notas de crédito
        public decimal ArticuloPrecio { get; set; } // decimal(19,2)
        #endregion

        #region VALORES  // Los "Resta" son para nota de crédito
        public decimal? SumaPesos { get; set; } // decimal(19,2)
        public decimal? RestaPesos { get; set; } // decimal(19,2), Notas de crédito
        public decimal? SumaDolares { get; set; } // decimal(19,2)
        public decimal? RestaDolares { get; set; } // decimal(19,2), Notas de crédito
        public decimal? Cambio { get; set; } // Tipo de cambio tomado, decimal(9,2)
        #endregion

        // Esta propiedad adicional es solo para mostrar el encabezado agrupado
        public int? FacturaID_dummy => FacturaID;

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
        public string? Descrip { get; set; }
        public string? Notas { get; set; }
        public bool Previsto { get; set; }
        public decimal SumaPesos { get; set; }
        public decimal RestaPesos { get; set; }
        public decimal Alicuota { get; set; }
        public int? MovimientoID { get; set; }
        public char Accion { get; set; }
    }

}