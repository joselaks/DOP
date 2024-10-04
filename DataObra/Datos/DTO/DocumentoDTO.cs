using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Datos.DTO
{
    public class DocumentoDTO
    {
        #region SISTEMA
        public int? ID { get; set; }  // Int o BigInt mas adelante
        public int CuentaID { get; set; } // SmallInt
        public int TipoID { get; set; } // TinyInt o 1 Char
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
        public DateTime Fecha1 { get; set; } // Date Fecha del docuemnto ingresada por el usuario
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

        #region Por Tipo
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
    }
}
