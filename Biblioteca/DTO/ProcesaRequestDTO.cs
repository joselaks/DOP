using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class ProcesaArticulosListaRequest
        {
        public ArticulosListaDTO Lista { get; set; }
        public List<ArticuloDTO> Articulos { get; set; }
        }

    public class ProcesarGastoRequest
        {
        public GastoDTO Gasto { get; set; } = new();
        public List<GastoDetalleDTO> Detalles { get; set; } = new();
        public List<int> PresupuestosAfectados { get; set; } = new();
        }

    // Resultado devuelto por Servidor.Repositorios.rDocumentos.ProcesarGastoAsync
    public class ProcesarGastoResult
        {
        public int DocumentoID { get; set; }
        public List<int> PresupuestoIDs { get; set; } = new();
        public List<PresupuestoResumen> Resumenes { get; set; } = new();
        }

    public class PresupuestoResumen
        {
        public int PresupuestoID { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public decimal TotalGasto { get; set; }
        public decimal TotalCobro { get; set; }
        }

    }