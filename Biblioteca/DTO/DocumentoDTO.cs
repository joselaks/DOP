using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class DocumentoDTO
        {
        public int ID { get; set; }
        public short CuentaID { get; set; }
        public byte TipoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime CreadoFecha { get; set; } // Cambiar a DateOnly si es necesario
        public int? EditadoID { get; set; }
        public DateTime? EditadoFecha { get; set; }
        public int? AutorizadoID { get; set; } // Agregado
        public DateTime? AutorizadoFecha { get; set; } // Agregado
        public int? AdminID { get; set; }
        public int? ObraID { get; set; }
        public int? PresupuestoID { get; set; }
        public int? RubroID { get; set; }
        public int? EntidadID { get; set; }
        public int? DepositoID { get; set; }
        public string? Descrip { get; set; } // VARCHAR(150)
        public string? Concepto1 { get; set; } // VARCHAR(150)
        public DateTime Fecha1 { get; set; } // Cambiar a DateOnly si es necesario
        public DateTime? Fecha2 { get; set; }
        public DateTime? Fecha3 { get; set; }
        public int? Numero1 { get; set; } // Cambiado a nullable
        public int? Numero2 { get; set; } // Cambiado a nullable
        public int? Numero3 { get; set; } // Cambiado a nullable
        public string? Notas { get; set; } // VARCHAR(250)
        public bool Active { get; set; }
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        public decimal Impuestos { get; set; }
        public decimal ImpuestosD { get; set; }
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
        public bool RelDoc { get; set; }
        public bool RelArt { get; set; }
        public bool RelMov { get; set; }
        public bool RelImp { get; set; }
        public bool RelRub { get; set; }
        public bool RelTar { get; set; }
        public bool RelIns { get; set; }
        }

    }