using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace Biblioteca
    {
    public class Control : ObjetoNotificable
        {
        public ObservableCollection<Nodo> Arbol { get; private set; } = new();
        // Colección de insumos acumulados (previstos y reales)
        public ObservableCollection<Nodo> Insumos { get; private set; } = new();
        private List<GastoDetalleDTO> GastosPresupuesto = new();

        public void Construir(List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, List<GastoDetalleDTO> detalles)
            {
            if (conceptos == null) conceptos = new List<ConceptoDTO>();
            if (relaciones == null) relaciones = new List<RelacionDTO>();
            if (detalles == null) detalles = new List<GastoDetalleDTO>();

            GastosPresupuesto = detalles;
            Arbol.Clear();

            var relacionesRaiz = relaciones
                .Where(r => r.CodSup == "0")
                .OrderBy(r => r.OrdenInt)
                .ToList();

            foreach (var rel in relacionesRaiz)
                {
                var rubro = conceptos.FirstOrDefault(c => c.ConceptoID == rel.CodInf);
                if (rubro == null) continue;

                var nodoRubro = new Nodo
                    {
                    ID = rubro.ConceptoID,
                    Descripcion = rubro.Descrip,
                    Tipo = (rubro.Tipo == '0') ? "R" : rubro.Tipo.ToString(),
                    Unidad = rubro.Unidad,
                    Cantidad = 1,
                    CantidadReal = 1,
                    Sup = true,
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                var hijos = GetElementosHijos(nodoRubro, conceptos, relaciones, 1);
                if (hijos != null && hijos.Count > 0)
                    nodoRubro.Inferiores = new ObservableCollection<Nodo>(hijos);

                Arbol.Add(nodoRubro);
                }

            // Recálculo inicial
            RecalculoCompleto();
            }

        private ObservableCollection<Nodo> GetElementosHijos(Nodo elemento, List<ConceptoDTO> listaConceptos, List<RelacionDTO> listaRelaciones, int nivel)
            {
            if (elemento == null) return null;

            var hijos = new ObservableCollection<Nodo>();
            var rels = listaRelaciones
                .Where(r => r.CodSup == elemento.ID)
                .OrderBy(r => r.OrdenInt)
                .ToList();

            foreach (var r in rels)
                {
                var c = listaConceptos.FirstOrDefault(x => x.ConceptoID == r.CodInf);
                if (c == null) continue;

                var n = new Nodo
                    {
                    ID = c.ConceptoID,
                    Descripcion = c.Descrip,
                    PU1 = c.PrEjec,
                    PU2 = c.PrEjec1,
                    Pur1 = c.PrReal ?? 0,
                    Cantidad = r.CanEjec,
                    CantidadReal = r.CanReal ?? 0,
                    Sup = false,
                    Unidad = c.Unidad,
                    Tipo = c.Tipo.ToString(),
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                if (elemento.Tipo == "T" && c.Tipo.ToString() == "T")
                    n.Tipo = "A";
                else
                    {
                    bool esUltimoNivel = !listaRelaciones.Any(x => x.CodSup == c.ConceptoID);
                    if (esUltimoNivel && !"MDES".Contains(c.Tipo))
                        n.Tipo = c.Tipo.ToString();
                    }

                var sub = GetElementosHijos(n, listaConceptos, listaRelaciones, nivel + 1);
                if (sub != null && sub.Count > 0)
                    n.Inferiores = new ObservableCollection<Nodo>(sub);

                hijos.Add(n);
                }
            return hijos;
            }

        public void RecalculoCompleto()
            {
            CalcularArbol(Arbol, true, 0m, 0m);
            InsertarGastosComoInferioresPorInsumo(Insumos);

            }

        private void CalcularArbol(IEnumerable<Nodo> items, bool inicio, decimal factorSup, decimal factorSupReal)
            {
            int orden = 1;

            if (inicio)
                {
                foreach (var n in items) n.OrdenInt = 0;

                // Limpiar Insumos completamente para evitar residuos
                Insumos?.Clear();
                if (Insumos == null)
                    Insumos = new ObservableCollection<Nodo>();
                }

            foreach (var item in items)
                {
                item.OrdenInt = (item.OrdenInt == 0) ? orden++ : item.OrdenInt;

                if (item.HasItems)
                    {
                    if (item.Sup)
                        {
                        // Inicializa factores en el superior
                        factorSup = item.Cantidad;
                        factorSupReal = item.CantidadReal;
                        }

                    // Descenso: propagar multiplicando ambos factores
                    CalcularArbol(item.Inferiores, false, factorSup * item.Cantidad, factorSupReal * item.CantidadReal);

                    item.Factor = factorSup;

                    var importeHijos = item.Inferiores.Sum(x => x.Importe1);
                    item.PU1 = importeHijos;
                    item.Importe1 = item.Cantidad * item.PU1;

                    item.ImporteReal1 = item.CantidadReal * item.Pur1;
                    }
                else
                    {
                    // Hoja: calcular importes previstos y reales del nodo
                    item.Importe1 = item.Cantidad * item.PU1;
                    item.ImporteReal1 = item.CantidadReal * item.Pur1;


                    // Acumular en Insumos
                    var existente = Insumos.FirstOrDefault(a => a.ID == item.ID);
                    if (existente == null)
                        {
                        var registro = new Nodo
                            {
                            ID = item.ID,
                            Descripcion = item.Descripcion,
                            Unidad = item.Unidad,
                            Tipo = item.Tipo,
                            // Cantidad prevista ponderada por factorSup
                            Cantidad = item.Cantidad * factorSup,
                            // CantidadReal controlada por factorSupReal (si 0 => 0)
                            //CantidadReal = 0,
                            PU1 = item.PU1,
                            Pur1 = item.Pur1,
                            Importe1 = (item.Cantidad * factorSup) * item.PU1,
                            //ImporteReal1 = 0,
                            Sup = false,
                            Inferiores = new ObservableCollection<Nodo>()
                            };
                        Insumos.Add(registro);
                        }
                    else
                        {
                        existente.Descripcion = item.Descripcion;
                        existente.Unidad = item.Unidad;
                        existente.PU1 = item.PU1;
                        existente.Pur1 = item.Pur1;
                        existente.Cantidad += (item.Cantidad * factorSup);
                        //// CantidadReal controlada por factorSupReal
                        existente.CantidadReal = 0;
                        existente.Tipo = item.Tipo;
                        existente.Importe1 = existente.PU1 * existente.Cantidad;
                        //existente.ImporteReal1 = existente.Pur1 * existente.CantidadReal;
                        }
                    }

                if (!item.HasItems)
                    item.Factor = factorSup;
                }
            }

        public void InsertarGastosComoInferioresPorInsumo(ObservableCollection<Nodo> Insumos)
            {
            if (GastosPresupuesto == null || GastosPresupuesto.Count == 0) return;

            foreach (var gasto in GastosPresupuesto)
                {
                Nodo nodoGasto = new Nodo
                    {
                    ID = gasto.ID.ToString(),
                    Descripcion = gasto.Descrip,
                    Unidad = gasto.Unidad,
                    Tipo = gasto.TipoID.ToString(),
                    Cantidad = 0,
                    CantidadReal = gasto.Cantidad,
                    Pur1 = gasto.PrecioUnitario,
                    Importe1 = 0m
                    };
                var insumoRelacionado = Insumos.FirstOrDefault(i => i.ID == gasto.InsumoID);
                if (insumoRelacionado != null)
                    {
                    // Agregar como inferior del insumo relacionado
                    if (insumoRelacionado.Inferiores == null)
                        insumoRelacionado.Inferiores = new ObservableCollection<Nodo>();
                    insumoRelacionado.Inferiores.Add(nodoGasto);
                    }
                else
                    {
                    // Si no existe el insumo, agregar el gasto directamente a la lista de insumos
                    Insumos.Add(nodoGasto);
                    }
                }
            }
        }
    }
