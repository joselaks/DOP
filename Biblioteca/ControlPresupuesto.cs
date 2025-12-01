using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Biblioteca
    {
    // Genera un árbol de presupuesto para control, agregando el Rubro "Insumos no imputados"
    public class ControlPresupuesto : ObjetoNotificable
        {
        public ObservableCollection<Nodo> Arbol { get; private set; } = new();

        // Colecciones derivadas (como en Presupuesto)
        public ObservableCollection<Nodo> Rubros { get; private set; } = new();
        public ObservableCollection<Nodo> Tareas { get; private set; } = new();
        public ObservableCollection<Nodo> Auxiliares { get; private set; } = new();

        // Construye el árbol base y agrega el Rubro "Insumos no imputados"
        public void Construir(List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, List<GastoDetalleDTO> detalles)
            {
            if (conceptos == null) conceptos = new List<ConceptoDTO>();
            if (relaciones == null) relaciones = new List<RelacionDTO>();
            if (detalles == null) detalles = new List<GastoDetalleDTO>();

            // 1) Armar árbol como en Presupuesto.generaPresupuesto (raíz = "0")
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
                    Sup = true,
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                // Hijos
                var hijos = GetElementosHijos(nodoRubro, conceptos, relaciones, 1);
                if (hijos != null && hijos.Count > 0)
                    {
                    nodoRubro.Inferiores = new ObservableCollection<Nodo>(hijos);
                    }

                Arbol.Add(nodoRubro);
                }

            // 2) Agregar rubro "Insumos no imputados" con un T por cada naturaleza en detalles.TipoID
            AgregarInsumosNoImputados(detalles);

            RecalculoCompleto();

            }

        // NUEVO: Procedimiento similar a Presupuesto.RecalculoCompleto
        public void RecalculoCompleto()
            {
            Recalcular(Arbol, true, 0);
            }

        // Recálculo recursivo (basado en Presupuesto.recalculo)
        private void Recalcular(IEnumerable<Nodo> items, bool inicio, decimal factorSup)
            {
            int orden = 1;

            if (inicio)
                {
                foreach (var n in items) n.OrdenInt = 0;
                }

            foreach (var item in items)
                {
                item.OrdenInt = (item.OrdenInt == 0) ? orden++ : item.OrdenInt;

                if (item.HasItems)
                    {
                    if (item.Sup)
                        factorSup = item.Cantidad;

                    Recalcular(item.Inferiores, false, factorSup * item.Cantidad);

                    item.Factor = factorSup;

                    // Consolidado
                    var importeHijos = item.Inferiores.Sum(x => x.Importe1);

                    // Igual que Presupuesto: PU1 = suma hijos y luego Importe1 = Cantidad * PU1
                    item.PU1 = importeHijos;
                    item.Importe1 = item.Cantidad * item.PU1;
                    }
                else
                    {
                    item.Importe1 = item.Cantidad * item.PU1;
                    }

                if (!item.HasItems)
                    item.Factor = factorSup;
                }
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

        private void AgregarInsumosNoImputados(List<GastoDetalleDTO> detalles)
            {
            var gruposNaturaleza = detalles
                .Where(d => d != null)
                .GroupBy(d => d.TipoID)
                .OrderBy(g => g.Key)
                .ToList();

            if (gruposNaturaleza.Count == 0)
                return;

            var rubroNoImputados = new Nodo
                {
                ID = "R-INP",
                Descripcion = "Insumos no imputados",
                Tipo = "R",
                Unidad = "Gl",
                Cantidad = 1,
                Sup = true,
                Inferiores = new ObservableCollection<Nodo>()
                };

            foreach (var g in gruposNaturaleza)
                {
                char nat = g.Key == '\0' ? 'O' : g.Key;
                string id = $"T-INP-{nat}";
                string desc = $"{NaturalezaDescripcion(nat)} no imputados";

                var nodoT = new Nodo
                    {
                    ID = id,
                    Descripcion = desc,
                    Tipo = "T",
                    Unidad = "Gl",
                    Cantidad = 1,
                    Sup = false,
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                // Agregar un nodo hoja por cada GastoDetalleDTO de la naturaleza
                var hijos = g
                    .Where(d => d != null)
                    .OrderBy(d => d.Descrip)
                    .Select(d => new Nodo
                        {
                        ID = $"I-INP-{nat}-{(d.InsumoID ?? d.ID.ToString())}",
                        Descripcion = d.Descrip ?? $"Insumo {d.ID}",
                        Tipo = nat.ToString(),          // Se usa la naturaleza para permitir consolidaciones por tipo
                        Unidad = d.Unidad ?? "Gl",
                        Cantidad = d.Cantidad,
                        PU1 = d.PrecioUnitario,
                        Sup = false,
                        Inferiores = new ObservableCollection<Nodo>()
                        });

                foreach (var h in hijos)
                    nodoT.Inferiores.Add(h);

                rubroNoImputados.Inferiores.Add(nodoT);
                }

            Arbol.Add(rubroNoImputados);
            }

        private static string NaturalezaDescripcion(char tipo)
            {
            return tipo switch
                {
                    'M' => "Materiales",
                    'D' => "Mano de obra",
                    'E' => "Equipos",
                    'S' => "Subcontratos",
                    'O' => "Otros",
                    'A' => "Auxiliares",
                    _ => $"Naturaleza {tipo}"
                    };
            }

        }
    }