using Bibioteca.Clases;
using Biblioteca.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Biblioteca
{
    public class Control1 : ObjetoNotificable
    {
        // Tipos anidados para que puedan referenciarse como Control1.ConceptoConGastosPropio y Control1.GastoPropio
        public class ConceptoConGastosPropio : ObjetoNotificable
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
            public decimal? CantTotalReal { get; set; }
            public decimal? CanTotalSaldo { get; set; }
            public decimal? Existencias { get; set; }

            private decimal _prReal;
            public decimal PrReal
            {
                get => _prReal;
                set { _prReal = value; OnPropertyChanged(nameof(PrReal)); }
            }

            public decimal? PrReal1 { get; set; }

            private decimal _importeTotalEjec;
            public decimal ImporteTotalEjec
            {
                get => _importeTotalEjec;
                set { _importeTotalEjec = value; OnPropertyChanged(nameof(ImporteTotalEjec)); }
            }

            public decimal ImporteTotalEjec1 { get; set; }

            private decimal _importeRealEjec;
            public decimal ImporteRealEjec
            {
                get => _importeRealEjec;
                set { _importeRealEjec = value; OnPropertyChanged(nameof(ImporteRealEjec)); }
            }

            public decimal ImporteRealEjec1 { get; set; }

            private decimal _importeSaldoEjec;
            public decimal ImporteSaldoEjec
            {
                get => _importeSaldoEjec;
                set { _importeSaldoEjec = value; OnPropertyChanged(nameof(ImporteSaldoEjec)); }
            }

            public decimal ImporteSaldoEjec1 { get; set; }
            public int? ArticuloID { get; set; }
            public decimal? FactorArticulo { get; set; }
            public char? Accion { get; set; }
            public bool imprevisto { get; set; }
            public ObservableCollection<GastoPropio> Gastos { get; set; } = new();
        }

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

        // Estado y acumuladores
        public ObservableCollection<ConceptoConGastosPropio> ConceptosConGastosPropios { get; set; } = new();
        public decimal TotalPreviso = 0;
        public decimal TotalReal = 0;
        public decimal TotalSaldo = 0;
        private List<GastoReasignacionDTO> asignacionInicial = new();
        private List<GastoReasignacionDTO> asignacionActual = new();
        public List<GastoReasignacionDTO> reAasignacion = new();

        // Construcción: además de armar el modelo, captura el estado original
        public void ConstruirConceptosConGastos(List<ConceptoDTO> conceptos, List<GastoDetalleDTO> detalles)
        {
            var tiposValidos = new HashSet<char> { 'M', 'D', 'S', 'O', 'E' };
            var conceptosFiltrados = conceptos
                .Where(c =>
                    c != null &&
                    tiposValidos.Contains(c.Tipo)
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
                    Gastos = new ObservableCollection<GastoPropio>(
                        detalles
                            .Where(d => d.InsumoID == item.ConceptoID)
                            .Select(d =>
                            {
                                var g = new GastoPropio
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
                                };
                                return g;
                            })
                    )
                };

                ConceptosConGastosPropios.Add(conceptoPropio);
            }

            AgregarNoImputados(detalles);
            LlenarAsignacionInicial();
            Recalculo();
        }

        // Llena la lista inicial de asignaciones a partir del estado actual del control
        public void LlenarAsignacionInicial()
            {
            asignacionInicial.Clear();

            foreach (var concepto in ConceptosConGastosPropios)
                {
                if (concepto.Gastos == null) continue;

                foreach (var gasto in concepto.Gastos)
                    {
                    var clave = gasto.ID;
                    asignacionInicial.Add(new GastoReasignacionDTO
                        {
                        GastoID = clave,
                        NuevoInsumoID = gasto.InsumoID
                        });
                    }
                }
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

                decimal suma = concepto.Gastos
                    .Where(g => g.FactorConcepto != 0)
                    .Sum(g => g.Cantidad / g.FactorConcepto);

                concepto.CantTotalReal = suma;
                concepto.CanTotalSaldo = concepto.CanTotalEjec - concepto.CantTotalReal;
                concepto.ImporteRealEjec = concepto.Gastos.Sum(g => g.Importe);
                concepto.PrReal = (concepto.CantTotalReal.HasValue && concepto.CantTotalReal.Value != 0)
                    ? concepto.ImporteRealEjec / concepto.CantTotalReal.Value
                    : 0;
                concepto.ImporteTotalEjec = concepto.PrEjec * concepto.CanTotalEjec;
                concepto.ImporteSaldoEjec = concepto.ImporteTotalEjec - concepto.ImporteRealEjec;
            }
            TotalPreviso = ConceptosConGastosPropios.Sum(c => c.ImporteTotalEjec);
            TotalReal = ConceptosConGastosPropios.Sum(c => c.ImporteRealEjec);
            TotalSaldo = ConceptosConGastosPropios.Sum(c => c.ImporteSaldoEjec);
            AjusteFinal();
        }

        // Llena la lista de asignaciones actuales a partir del estado actual del control
        public void LlenarAsignacionActual()
            {
            asignacionActual.Clear();

            foreach (var concepto in ConceptosConGastosPropios)
                {
                if (concepto.Gastos == null) continue;

                foreach (var gasto in concepto.Gastos)
                    {
                    var clave = gasto.ID;
                    asignacionActual.Add(new GastoReasignacionDTO
                        {
                        GastoID = clave,
                        NuevoInsumoID = gasto.InsumoID
                        });
                    }
                }
            }

        // Compara asignacionActual contra asignacionInicial y genera reAasignacion con las diferencias
        public void GenerarReasignacionDesdeComparacion()
            {
            // Asegurar que haya estado actual
            if (asignacionActual.Count == 0)
                LlenarAsignacionActual();

            reAasignacion.Clear();

            foreach (var act in asignacionActual)
                {
                // Si no existe en inicial un registro igual (mismo GastoID y mismo NuevoInsumoID), agregar
                if (!asignacionInicial.Any(init =>
                        init.GastoID == act.GastoID &&
                        string.Equals(init.NuevoInsumoID, act.NuevoInsumoID, StringComparison.Ordinal)))
                    {
                    reAasignacion.Add(new GastoReasignacionDTO
                        {
                        GastoID = act.GastoID,
                        NuevoInsumoID = act.NuevoInsumoID
                        });
                    }
                }
            }

        private void AgregarNoImputados(List<GastoDetalleDTO> detalles)
        {
            var idsAsociados = ConceptosConGastosPropios
                .Select(c => c.ConceptoID)
                .ToHashSet();

            var gastosNoImputados = detalles
                .Where(g => string.IsNullOrWhiteSpace(g.InsumoID) || !idsAsociados.Contains(g.InsumoID))
                .ToList();

            if (gastosNoImputados.Count == 0)
                return;

            var agrupados = gastosNoImputados
                .GroupBy(g => g.InsumoID ?? "SIN_ID")
                .ToList();

            foreach (var grupo in agrupados)
            {
                var primerGasto = grupo.First();
                var conceptoNuevo = new ConceptoConGastosPropio
                {
                    ConceptoID = grupo.Key,
                    Descrip = primerGasto.Descrip,
                    Tipo = 'O',
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
                    Gastos = new ObservableCollection<GastoPropio>(
                        grupo.Select(g =>
                        {
                            var gp = new GastoPropio
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
                            };

                            var clave = g.GastoID ?? g.ID;

                            return gp;
                        })
                    )
                };

                ConceptosConGastosPropios.Add(conceptoNuevo);
            }
        }

        public void LimpiarReasignaciones() => asignacionInicial.Clear();

        private void AjusteFinal()
        {
            foreach (var concepto in ConceptosConGastosPropios)
            {
                if (!string.IsNullOrEmpty(concepto.Unidad) && concepto.Unidad.Equals("gl", StringComparison.OrdinalIgnoreCase))
                {
                    concepto.CantTotalReal = null;
                    concepto.CanTotalSaldo = null;
                    concepto.PrReal = concepto.ImporteRealEjec;
                }
            }

            for (int i = ConceptosConGastosPropios.Count - 1; i >= 0; i--)
            {
                var concepto = ConceptosConGastosPropios[i];
                if (concepto.ConceptoID == "SIN_ID" && (concepto.Gastos == null || concepto.Gastos.Count == 0))
                {
                    ConceptosConGastosPropios.RemoveAt(i);
                }
            }
        }
    }
}