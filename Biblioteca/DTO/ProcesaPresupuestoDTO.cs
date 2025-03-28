using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ProcesaPresupuestoDTO
        {
        public int PresupuestoID { get; set; }
        public List<ConceptoDTO> ListaConceptos { get; set; }
        public List<RelacionDTO> ListaRelaciones { get; set; }
        }
    }

