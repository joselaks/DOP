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
    public class ProcesaInsumoRequest
        {
        public InsumoDTO Insumo { get; set; }
        public List<ArticuloRelDTO> Articulos { get; set; }
        }

    // DTO para procesar lista de artículos y sus artículos
    public class ProcesaArticulosListaRequest
        {
        public ArticulosListaDTO Lista { get; set; }
        public List<ArticuloDTO> Articulos { get; set; }
        }

    public class ProcesaTareaMaestroRequest
        {
        public int UsuarioID { get; set; }
        public List<ConceptoMDTO> Conceptos { get; set; }
        public List<RelacionMDTO> Relaciones { get; set; }
        }

    public class ProcesarGastoRequest
        {
        public GastoDTO Gasto { get; set; }
        public List<GastoDetalleDTO> Detalles { get; set; }
        }


    }

