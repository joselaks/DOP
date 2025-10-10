using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class GastoDetalleDTO 
        {
        public int ID { get; set; }
        public int GastoID { get; set; }
        public int UsuarioID { get; set; }
        public char TipoID { get; set; }
        public int? PresupuestoID { get; set; }
        public string? RubroID { get; set; }
        public string? TareaID { get; set; }
        public string? AuxiliarID { get; set; }
        public string? InsumoID { get; set; }
        public string? Descrip { get; set; }
        public string? Unidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public char Moneda { get; set; }
        public decimal Importe { get; set; } = 0;
        public char Accion { get; set; }
        }
    }
