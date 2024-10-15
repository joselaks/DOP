using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Insumos.Clases
{
    // Tabla MaestroInsumos
    public class MaestroInsumo  //  Del Usuario, creados manualmente o automaticamente al Comprar/Facturar
    {
        #region SISTEMA
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; } // Creador
        public int EditadoID { get; set; } // Date Ultima edición
        public DateTime Editado { get; set; }
        #endregion
        #region DATOS
        public int TipoID { get; set; } // char(1) Materiales Mano de Obra, etc.
        public string Descrip { get; set; } = string.Empty; // varchar 65
        public string Unidad { get; set; } = string.Empty; // char 2
        public string? Codigo { get; set; } // varchar 20
        #endregion
        #region VALOR
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        #endregion
        #region DE DOCUMENTOS USUARIO
        public int? FacturaID { get; set; }  // De la ultima Factura en la que se compro este Insumo
        public DateTime? FacturaFecha { get; set; }
        public decimal? FacturaPesos { get; set; }
        public decimal? FacturaDolares { get; set; }
        public int? CompraID { get; set; }  // De la ultima Compra en la que se compro este Insumo
        public DateTime? CompraFecha { get; set; }
        public decimal? CompraPesos { get; set; }
        public decimal? CompraDolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelArtDO { get; set; } // Si tiene relación con articulos del maestro de DO
        public bool RelArtUsr { get; set; } // Si tiene relación con articulos del maestro del Usuario
        #endregion
    }
}
