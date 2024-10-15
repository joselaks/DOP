using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos
{
    public class DocumentoDet  // Tabla DocumentosDet
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Editado { get; set; } // date
        public char TipoID { get; set; } // Insumos Dto Acopio Dto Anticipo Det Certificado
        #endregion
        #region AGRUPADORES
        public int? AdminID { get; set; }
        public int? EntidadID { get; set; }

        // public int? EntidadTipo { get; set; } no seria necesario
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
        public int? RubroID { get; set; }
        public int? TareaID { get; set; }
        #endregion
        #region DATOS ARTICULO
        public DateTime Fecha { get; set; }
        public string ArticuloDescrip { get; set; } = string.Empty; // varchar65
        public decimal? ArticuloCantSuma { get; set; } // decimal 19,2
        public decimal? ArticuloCantResta { get; set; } // Notas de credito
        public decimal ArticuloPrecio { get; set; } // decimal 19,2
        #endregion
        #region VALORES  // Los resta son para nota de crédito
        public decimal? SumaPesos { get; set; } // decimal 19,2
        public decimal? RestaPesos { get; set; } // decimal 19,2  Notas de credito
        public decimal? SumaDolares { get; set; } // decimal 19,2
        public decimal? RestaDolares { get; set; } // decimal 19,2  Notas de credito
        public decimal? Cambio { get; set; } // Tipo de cambio tomado decimal 9,2
        #endregion
    }
}
