using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos.Clases
{
    public class Documento
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int CuentaID { get; set; }
        public int TipoID { get; set; } // Tipo de Documento
        public string? TipoDoc { get; set; } // Tipo en texto
        #endregion
        #region USUARIOS
        public int UsuarioID { get; set; } // Creador inicial
        public string? Usuario { get; set; }
        public DateTime CreadoFecha { get; set; }
        public int EditadoID { get; set; } // Ultimo en editar
        public string? Editado { get; set; }
        public DateTime EditadoFecha { get; set; }
        public int RevisadoID { get; set; } // Usuario que Autoriza o Verifica
        public string? Revisado { get; set; }
        public DateTime RevisadoFecha { get; set; }
        #endregion
        #region AGRUPADORES
        public int? AdminID { get; set; }
        public string? Admin { get; set; }
        public int? ObraID { get; set; }
        public string? Obra { get; set; }
        public int? PresupuestoID { get; set; }
        public string? Presupuesto { get; set; }
        public int? RubroID { get; set; }
        public string? Rubro { get; set; }
        public int? EntidadID { get; set; }
        public string? Entidad { get; set; }
        public string? EntidadTipo { get; set; }
        public int? DepositoID { get; set; }
        public string? Deposito { get; set; }
        #endregion
        #region DATOS
        public string? Descrip { get; set; }
        public string? Concepto1 { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime? Fecha2 { get; set; }
        public DateTime? Fecha3 { get; set; }
        public int Numero1 { get; set; }
        public int Numero2 { get; set; }
        public int Numero3 { get; set; }
        public string? Notas { get; set; }

        public bool Active { get; set; }
        #endregion
        #region TOTALES
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        public decimal Impuestos { get; set; }
        public decimal ImpuestosD { get; set; } // ¿?

        #region Por Tipo
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

    class DocumentoRel
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public int CuentaID { get; set; }

        public bool PorInsumos { get; set; } = true; // Si la relación esta dada por compartir detalle de insumos
                                                     // o es manual
    }
}
