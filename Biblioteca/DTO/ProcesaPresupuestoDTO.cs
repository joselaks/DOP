using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
    {
    public class ProcesaPresupuestoRequest
        {
        public PresupuestoDTO Presupuesto { get; set; }
        public List<ConceptoDTO> Conceptos { get; set; }
        public List<RelacionDTO> Relaciones { get; set; }
        }

    public class ConceptosRelacionesResult
        {
        public List<ConceptoDTO> Conceptos { get; set; }
        public List<RelacionDTO> Relaciones { get; set; }
        }

    }

