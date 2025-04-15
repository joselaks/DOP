using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class DocumentoDetDTO
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
        public string? RubroID { get; set; } // Cambiado de int? a string? para reflejar VARCHAR(20)
        public string? TareaID { get; set; } // Cambiado de int? a string? para reflejar VARCHAR(20)
        public string? InsumoID { get; set; } // Nuevo campo agregado como string? para reflejar VARCHAR(20)
        public DateTime? Fecha { get; set; }
        public string ArticuloDescrip { get; set; } = string.Empty; // Default para reflejar la restricción DEFAULT ('')
        public decimal ArticuloCantSuma { get; set; } = 0; // Default para reflejar la restricción DEFAULT ((0))
        public decimal ArticuloCantResta { get; set; } = 0; // Default para reflejar la restricción DEFAULT ((0))
        public decimal ArticuloPrecio { get; set; }
        public decimal? SumaPesos { get; set; }
        public decimal? RestaPesos { get; set; }
        public decimal? SumaDolares { get; set; }
        public decimal? RestaDolares { get; set; }
        public decimal? Cambio { get; set; }
        public char Accion { get; set; }
        }
    }
