using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int EditadoID { get; set; } // Int Ultimo usuario en editar
        public string? Editado { get; set; } // Mostrar usuario
        public DateTime EditadoFecha { get; set; } // Date Ultima modificación
        public int RevisadoID { get; set; } // Int Usuario que Autoriza o Verifica
        public string? Revisado { get; set; } // Mostrar usuario
        public DateTime RevisadoFecha { get; set; } // Date Cuando fue revisado
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
        public string? Concepto1 { get; set; } // varchar150
        public DateTime Fecha1 { get; set; } // Date Fecha del documento ingresada por el usuario
        public DateTime? Fecha2 { get; set; } // Date Fecha de vencimiento o entrega o ...
        public DateTime? Fecha3 { get; set; } // Date Fecha contable o ...
        public int Numero1 { get; set; } // Int en las facturas en numero de sucursal
        public int Numero2 { get; set; } // Int 
        public int Numero3 { get; set; } // Int numero de archivo u otro
        public string? Notas { get; set; } // Varchar250 Ingresadas por el usuario
        public bool Active { get; set; } // Bool
        #endregion
        #region TOTALES
        public decimal Pesos { get; set; } // Decimal 19.2
        public decimal Dolares { get; set; } // Decimal 19.2
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
        public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra

        public List<DocumentoDet> DetalleDocumento { get; set; }
        public List<Movimiento> DetalleMovimientos { get; set; }
        public List<Impuesto> DetalleImpuestos { get; set; }

        #endregion

        #region METODOS
        public static Documento Convertir(Biblioteca.DTO.DocumentoDTO docDTO)
        {
            return new Documento
            {
                ID = docDTO.ID,
                CuentaID = docDTO.CuentaID,
                TipoID = docDTO.TipoID,
                UsuarioID = docDTO.UsuarioID,
                CreadoFecha = docDTO.CreadoFecha,
                EditadoID = docDTO.EditadoID,
                EditadoFecha = docDTO.EditadoFecha,
                RevisadoID = docDTO.RevisadoID,
                RevisadoFecha = docDTO.RevisadoFecha,
                AdminID = docDTO.AdminID,
                ObraID = docDTO.ObraID,
                PresupuestoID = docDTO.PresupuestoID,
                RubroID = docDTO.RubroID,
                EntidadID = docDTO.EntidadID,
                DepositoID = docDTO.DepositoID,
                Descrip = docDTO.Descrip,
                Concepto1 = docDTO.Concepto1,
                Fecha1 = docDTO.Fecha1,
                Fecha2 = docDTO.Fecha2,
                Fecha3 = docDTO.Fecha3,
                Numero1 = docDTO.Numero1,
                Numero2 = docDTO.Numero2,
                Numero3 = docDTO.Numero3,
                Notas = docDTO.Notas,
                Active = docDTO.Active,
                Pesos = docDTO.Pesos,
                Dolares = docDTO.Dolares,
                Impuestos = docDTO.Impuestos,
                ImpuestosD = docDTO.ImpuestosD,
                Materiales = docDTO.Materiales,
                ManodeObra = docDTO.ManodeObra,
                Subcontratos = docDTO.Subcontratos,
                Equipos = docDTO.Equipos,
                Otros = docDTO.Otros,
                MaterialesD = docDTO.MaterialesD,
                ManodeObraD = docDTO.ManodeObraD,
                SubcontratosD = docDTO.SubcontratosD,
                EquiposD = docDTO.EquiposD,
                OtrosD = docDTO.OtrosD,
                RelDoc = docDTO.RelDoc,
                RelArt = docDTO.RelArt,
                RelMov = docDTO.RelMov,
                RelImp = docDTO.RelImp,
                RelRub = docDTO.RelRub,
                RelTar = docDTO.RelTar,
                RelIns = docDTO.RelIns
            };
        }

        public static Biblioteca.Documento ConvertirInverso(Documento doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            return new Biblioteca.Documento
            {
                ID = doc.ID,
                CuentaID = doc.CuentaID,
                TipoID = doc.TipoID,
                UsuarioID = doc.UsuarioID,
                CreadoFecha = doc.CreadoFecha,
                EditadoID = doc.EditadoID,
                EditadoFecha = doc.EditadoFecha,
                RevisadoID = doc.RevisadoID,
                RevisadoFecha = doc.RevisadoFecha,
                AdminID = doc.AdminID,
                ObraID = doc.ObraID,
                PresupuestoID = doc.PresupuestoID,
                RubroID = doc.RubroID,
                EntidadID = doc.EntidadID,
                DepositoID = doc.DepositoID,
                Descrip = doc.Descrip,
                Concepto1 = doc.Concepto1,
                Fecha1 = doc.Fecha1,
                Fecha2 = doc.Fecha2,
                Fecha3 = doc.Fecha3,
                Numero1 = doc.Numero1,
                Numero2 = doc.Numero2,
                Numero3 = doc.Numero3,
                Notas = doc.Notas,
                Active = doc.Active,
                Pesos = doc.Pesos,
                Dolares = doc.Dolares,
                Impuestos = doc.Impuestos,
                ImpuestosD = doc.ImpuestosD,
                Materiales = doc.Materiales,
                ManodeObra = doc.ManodeObra,
                Subcontratos = doc.Subcontratos,
                Equipos = doc.Equipos,
                Otros = doc.Otros,
                MaterialesD = doc.MaterialesD,
                ManodeObraD = doc.ManodeObraD,
                SubcontratosD = doc.SubcontratosD,
                EquiposD = doc.EquiposD,
                OtrosD = doc.OtrosD,
                RelDoc = doc.RelDoc,
                RelArt = doc.RelArt,
                RelMov = doc.RelMov,
                RelImp = doc.RelImp,
                RelRub = doc.RelRub,
                RelTar = doc.RelTar,
                RelIns = doc.RelIns
            };
        }
        
        #endregion
    }
    //public class Documento
    //{
    //    public int? ID { get; set; }
    //    public short CuentaID { get; set; }
    //    public byte TipoID { get; set; }
    //    public string? Tipo { get; set; }
    //    public int UsuarioID { get; set; }
    //    public string? Usuario { get; set; }
    //    public DateTime CreadoFecha { get; set; }
    //    public int EditadoID { get; set; }
    //    public string? Editado { get; set; }
    //    public DateTime EditadoFecha { get; set; }
    //    public int RevisadoID { get; set; }
    //    public string? Revisado { get; set; }
    //    public DateTime RevisadoFecha { get; set; }
    //    public int? AdminID { get; set; }
    //    public string? Admin { get; set; }
    //    public int? ObraID { get; set; }
    //    public string? Obra { get; set; }
    //    public int? PresupuestoID { get; set; }
    //    public string? Presupuesto { get; set; }
    //    public int? RubroID { get; set; }
    //    public string? Rubro { get; set; }
    //    public int? EntidadID { get; set; }
    //    public string? Entidad { get; set; }
    //    public string? EntidadTipo { get; set; }
    //    public int? DepositoID { get; set; }
    //    public string? Deposito { get; set; }
    //    public string Descrip { get; set; }
    //    public string? Concepto1 { get; set; }
    //    public DateTime Fecha1 { get; set; }
    //    public DateTime? Fecha2 { get; set; }
    //    public DateTime? Fecha3 { get; set; }
    //    public int Numero1 { get; set; }
    //    public int Numero2 { get; set; }
    //    public int Numero3 { get; set; }
    //    public string? Notas { get; set; }
    //    public bool Active { get; set; }
    //    public decimal Pesos { get; set; }
    //    public decimal Dolares { get; set; }
    //    public decimal Impuestos { get; set; }
    //    public decimal ImpuestosD { get; set; }
    //    public decimal Materiales { get; set; }
    //    public decimal ManodeObra { get; set; }
    //    public decimal Subcontratos { get; set; }
    //    public decimal Equipos { get; set; }
    //    public decimal Otros { get; set; }
    //    public decimal MaterialesD { get; set; }
    //    public decimal ManodeObraD { get; set; }
    //    public decimal SubcontratosD { get; set; }
    //    public decimal EquiposD { get; set; }
    //    public decimal OtrosD { get; set; }
    //    public bool RelDoc { get; set; }
    //    public bool RelArt { get; set; }
    //    public bool RelMov { get; set; }
    //    public bool RelImp { get; set; }
    //    public bool RelRub { get; set; }
    //    public bool RelTar { get; set; }
    //    public bool RelIns { get; set; }
    //    public char Accion { get; set; } // Campo Accion para definir si se agrega, modifica o borra

    //    public List<DocumentoDet> DetalleDocumento { get; set; }
    //    public List<Movimiento> DetalleMovimientos { get; set; }
    //    public List<Impuesto> DetalleImpuestos { get; set; }
    //}

    public class InfoDocumento
    {
        public List<DocumentoDet> DetalleDocumento { get; set; }
        public List<Movimiento> DetalleMovimientos { get; set; }
        public List<Impuesto> DetalleImpuestos { get; set; }

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


