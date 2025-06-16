using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class PresupuestoDTO
        {
        public int ID { get; set; }
        public int? CuentaID { get; set; }
        public int UsuarioID { get; set; }
        public string Descrip { get; set; }
        public decimal PrEjecTotal { get; set; }
        public decimal PrEjecDirecto { get; set; }
        public char EjecMoneda { get; set; }
        public decimal PrVentaTotal { get; set; }
        public decimal PrVentaDirecto { get; set; }
        public char VentaMoneda { get; set; }
        public decimal? Superficie { get; set; }
        public DateTime MesBase { get; set; }
        public DateTime FechaC { get; set; }
        public DateTime FechaM { get; set; }
        public bool EsModelo { get; set; }
        public decimal TipoCambioD { get; set; }
        }

    }
