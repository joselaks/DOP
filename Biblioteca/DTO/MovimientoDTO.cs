using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class MovimientoDTO
    {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public int TipoID { get; set; }
        public int? TesoreriaID { get; set; }
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? EntidadID { get; set; }
        public int? CompraID { get; set; }
        public int? ContratoID { get; set; }
        public int? FacturaID { get; set; }
        public int? GastoID { get; set; }
        public int? OrdenID { get; set; }
        public int? CobroID { get; set; }
        public int? PagoID { get; set; }
        public int? ImpuestoID { get; set; }
        public string Descrip { get; set; }
        public int Comprobante { get; set; }
        public int? Numero { get; set; }
        public string Notas { get; set; }
        public bool ChequeProcesado { get; set; }
        public bool Previsto { get; set; }
        public bool Desdoblado { get; set; }
        public decimal Cambio { get; set; }
        public bool RelMov { get; set; }
        public char Accion { get; set; }
    }

}
