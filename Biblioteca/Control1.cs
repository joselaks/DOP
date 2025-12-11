using Bibioteca.Clases;
using Biblioteca.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Biblioteca
    {
    public class Control1 : ObjetoNotificable
        {
        // Nueva lista con objetos propios, no derivados de ConceptoDTO ni GastoDetalleDTO
        public List<ConceptoConGastosPropio> ConceptosConGastosPropios { get; set; } = new();
        public decimal TotalPreviso = 0;
        public decimal TotalReal = 0;
        public decimal TotalSaldo = 0;

        // Recibe los DTOs pero transforma a objetos propios
        public void ConstruirConceptosConGastos(List<ConceptoDTO> conceptos, List<GastoDetalleDTO> detalles)
            {

            var tiposValidos = new HashSet<char> { 'M', 'D', 'S', 'O', 'E' };
            var conceptosFiltrados = conceptos
                .Where(c =>
                    c != null &&
                    tiposValidos.Contains(c.Tipo) // No uses ToUpperInvariant si los datos ya vienen en mayúscula
                    && c.Tipo != '\0'
                    && c.Tipo != ' '
                    && !string.IsNullOrWhiteSpace(c.ConceptoID)
                )
                .ToList();

            ConceptosConGastosPropios.Clear();

            foreach (var item in conceptosFiltrados)
                {
                var conceptoPropio = new ConceptoConGastosPropio
                    {
                    ConceptoID = item.ConceptoID,
                    Descrip = item.Descrip,
                    Tipo = item.Tipo,
                    Unidad = item.Unidad,
                    PrEjec = item.PrEjec,
                    PrEjec1 = item.PrEjec1,
                    EjecMoneda = item.EjecMoneda,
                    MesBase = item.MesBase,
                    CanTotalEjec = item.CanTotalEjec,
                    CantTotalReal = 0,
                    Existencias = item.Existencias,
                    PrReal = 0,
                    PrReal1 = item.PrReal1,
                    ArticuloID = item.ArticuloID,
                    FactorArticulo = item.FactorArticulo,
                    imprevisto = false,
                    Accion = item.Accion,
                    Gastos = detalles
                        .Where(d => d.InsumoID == item.ConceptoID)
                        .Select(d => new GastoPropio
                            {
                            ID = d.ID,
                            GastoID = d.GastoID,
                            CobroID = d.CobroID,
                            UsuarioID = d.UsuarioID,
                            CuentaID = d.CuentaID,
                            TipoID = d.TipoID,
                            InsumoID = d.InsumoID,
                            TareaID = d.TareaID,
                            UnicoUso = d.UnicoUso,
                            Descrip = d.Descrip,
                            Unidad = d.Unidad,
                            Cantidad = d.Cantidad,
                            FactorConcepto = d.FactorConcepto,
                            PrecioUnitario = d.PrecioUnitario,
                            Importe = d.Importe,
                            ArticuloID = d.ArticuloID,
                            Moneda = d.Moneda,
                            TipoCambioD = d.TipoCambioD,
                            Fecha = d.Fecha,
                            Accion = d.Accion
                            })
                        .ToList()
                    };

                ConceptosConGastosPropios.Add(conceptoPropio);
                }
            AgregarNoImputados(detalles);
            Recalculo ();
            }
        public void Recalculo()
            {
            foreach (var concepto in ConceptosConGastosPropios)
                {
                if (concepto.Gastos == null || concepto.Gastos.Count == 0)
                    {
                    concepto.CantTotalReal = 0;
                    concepto.CanTotalSaldo = concepto.CanTotalEjec - concepto.CantTotalReal;
                    concepto.ImporteRealEjec = 0;
                    concepto.PrReal = 0;
                    concepto.ImporteTotalEjec = concepto.PrEjec * concepto.CanTotalEjec;
                    concepto.ImporteSaldoEjec = concepto.ImporteTotalEjec;

                    continue;
                    }

                // Sumar (Cantidad / FactorConcepto) de cada gasto
                decimal suma = concepto.Gastos
                    .Where(g => g.FactorConcepto != 0) // Evita división por cero
                    .Sum(g => g.Cantidad / g.FactorConcepto);

                concepto.CantTotalReal = suma;
                concepto.CanTotalSaldo = concepto.CanTotalEjec - concepto.CantTotalReal ;
                concepto.ImporteRealEjec = concepto.Gastos.Sum(g => g.Importe);
                concepto.PrReal = concepto.CantTotalReal != 0 ? concepto.ImporteRealEjec / concepto.CantTotalReal : 0;

                concepto.ImporteTotalEjec = concepto.PrEjec * concepto.CanTotalEjec;

                concepto.ImporteSaldoEjec = concepto.ImporteTotalEjec - concepto.ImporteRealEjec;
                }
            TotalPreviso = ConceptosConGastosPropios.Sum(c => c.ImporteTotalEjec);
            TotalReal = ConceptosConGastosPropios.Sum(c => c.ImporteRealEjec);
            TotalSaldo = ConceptosConGastosPropios.Sum(c => c.ImporteSaldoEjec);
            }
        private void AgregarNoImputados(List<GastoDetalleDTO> detalles)
            {
            // Obtener todos los ConceptoID ya asociados
            var idsAsociados = ConceptosConGastosPropios
                .Select(c => c.ConceptoID)
                .ToHashSet();

            // Buscar todos los gastos no imputados (InsumoID no asociado a ningún ConceptoID)
            var gastosNoImputados = detalles
                .Where(g => string.IsNullOrWhiteSpace(g.InsumoID) || !idsAsociados.Contains(g.InsumoID))
                .ToList();

            if (gastosNoImputados.Count == 0)
                return;

            // Agrupar los gastos no imputados por InsumoID para crear Conceptos imprevistos
            var agrupados = gastosNoImputados
                .GroupBy(g => g.InsumoID ?? "SIN_ID")
                .ToList();

            foreach (var grupo in agrupados)
                {
                var primerGasto = grupo.First();
                ConceptosConGastosPropios.Add(new ConceptoConGastosPropio
                    {
                    ConceptoID = grupo.Key,
                    Descrip = primerGasto.Descrip,
                    Tipo = 'O', // O de "Otros"
                    Unidad = primerGasto.Unidad,
                    PrEjec = 0,
                    PrEjec1 = 0,
                    EjecMoneda = primerGasto.Moneda,
                    MesBase = primerGasto.Fecha ?? DateTime.MinValue,
                    CanTotalEjec = 0,
                    CantTotalReal = 0,
                    Existencias = null,
                    PrReal = 0,
                    PrReal1 = null,
                    ArticuloID = primerGasto.ArticuloID,
                    FactorArticulo = null,
                    Accion = primerGasto.Accion,
                    imprevisto = true,
                    Gastos = grupo.Select(g => new GastoPropio
                        {
                        ID = g.ID,
                        GastoID = g.GastoID,
                        CobroID = g.CobroID,
                        UsuarioID = g.UsuarioID,
                        CuentaID = g.CuentaID,
                        TipoID = g.TipoID,
                        PresupuestoID = g.PresupuestoID,
                        InsumoID = g.InsumoID,
                        TareaID = g.TareaID,
                        UnicoUso = g.UnicoUso,
                        Descrip = g.Descrip,
                        Unidad = g.Unidad,
                        Cantidad = g.Cantidad,
                        FactorConcepto = g.FactorConcepto,
                        PrecioUnitario = g.PrecioUnitario,
                        Importe = g.Importe,
                        ArticuloID = g.ArticuloID,
                        Moneda = g.Moneda,
                        TipoCambioD = g.TipoCambioD,
                        Fecha = g.Fecha,
                        Accion = g.Accion
                        }).ToList()
                    });
                }
            }

        // Objeto propio para concepto con gastos, no hereda de ConceptoDTO
        public class ConceptoConGastosPropio 
            {
            public string ConceptoID { get; set; }
            public string Descrip { get; set; }
            public char Tipo { get; set; }
            public string Unidad { get; set; }
            public decimal PrEjec { get; set; }
            public decimal PrEjec1 { get; set; }
            public char EjecMoneda { get; set; }
            public DateTime MesBase { get; set; }
            public decimal CanTotalEjec { get; set; }
            public decimal CantTotalReal { get; set; }
            public decimal CanTotalSaldo { get; set; }
            public decimal? Existencias { get; set; }
            public decimal PrReal { get; set; }
            public decimal? PrReal1 { get; set; }
            public decimal ImporteTotalEjec { get; set; }
            public decimal ImporteTotalEjec1 { get; set; }
            public decimal ImporteRealEjec { get; set; }
            public decimal ImporteRealEjec1 { get; set; }
            public decimal ImporteSaldoEjec { get; set; }
            public decimal ImporteSaldoEjec1 { get; set; }
            public int? ArticuloID { get; set; }
            public decimal? FactorArticulo { get; set; }
            public char? Accion { get; set; }
            public bool imprevisto { get; set; }
            public List<GastoPropio> Gastos { get; set; } = new();
            }

        // Objeto propio para gasto, no hereda de GastoDetalleDTO ni GastoDTO
        public class GastoPropio
            {
            public int ID { get; set; }
            public int? GastoID { get; set; }
            public int? CobroID { get; set; }
            public int UsuarioID { get; set; }
            public int CuentaID { get; set; }
            public char TipoID { get; set; }
            public int? PresupuestoID { get; set; }
            public string? InsumoID { get; set; }
            public string? TareaID { get; set; }
            public bool? UnicoUso { get; set; }
            public string Descrip { get; set; }
            public string Unidad { get; set; }
            public decimal Cantidad { get; set; }
            public decimal FactorConcepto { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Importe { get; set; }
            public int? ArticuloID { get; set; }
            public char Moneda { get; set; }
            public decimal TipoCambioD { get; set; }
            public DateTime? Fecha { get; set; }
            public char Accion { get; set; }
            }
        }
    }