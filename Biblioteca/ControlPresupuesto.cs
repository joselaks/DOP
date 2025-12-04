using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace Biblioteca
    {
    public class ControlPresupuesto : ObjetoNotificable
        {
        public ObservableCollection<Nodo> Arbol { get; private set; } = new();

        public ObservableCollection<Nodo> Rubros { get; private set; } = new();
        public ObservableCollection<Nodo> Tareas { get; private set; } = new();
        public ObservableCollection<Nodo> Auxiliares { get; private set; } = new();

        private List<GastoDetalleDTO> _detallesRelacionados = new();

        // NUEVO: control de recálculo y nodos observados
        private bool _recalcEnProgreso = false;
        private readonly HashSet<Nodo> _nodosObservados = new();

        public void Construir(List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, List<GastoDetalleDTO> detalles)
            {
            if (conceptos == null) conceptos = new List<ConceptoDTO>();
            if (relaciones == null) relaciones = new List<RelacionDTO>();
            if (detalles == null) detalles = new List<GastoDetalleDTO>();

            _detallesRelacionados = detalles;
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

            AgregarInsumosNoImputados(detalles);

            // Recálculo inicial
            RecalculoCompleto();
            }

        // NUEVO: recálculo completo público reutilizable
        public void RecalculoCompleto()
            {
            if (_recalcEnProgreso) return;
            try
                {
                _recalcEnProgreso = true;
                var cantidadesPorInsumo = ObtenerIdsInsumosDeDetalles();
                var idsSolo = cantidadesPorInsumo.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
                var nodosConGastos = SeleccionarNodosConInferioresEnIds(Arbol, idsSolo, incluirDescendientes: true);
                RedistribuirCantidadesPorInsumo(cantidadesPorInsumo, nodosConGastos);
                AsignarPrecioReal();
                CalcularArbol(Arbol, true, 0);
                }
            finally
                {
                _recalcEnProgreso = false;
                }
            }

        // Handler: si cambia CantidadReal de tarea (Tipo="T"), recalcular redistribución
        private void Nodo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
            if (sender is not Nodo nodo) return;
            if (_recalcEnProgreso) return;
            if (e.PropertyName != nameof(Nodo.CantidadReal)) return;
            if (!string.Equals(nodo.Tipo, "T", StringComparison.OrdinalIgnoreCase)) return;

            RecalculoCompleto();
            }

        private void AsignarPrecioReal()
            {
            var acumuladores = new Dictionary<string, (decimal cant, decimal imp)>(StringComparer.OrdinalIgnoreCase);

            if (_detallesRelacionados != null && _detallesRelacionados.Count > 0)
                {
                foreach (var d in _detallesRelacionados)
                    {
                    var id = d?.InsumoID?.Trim();
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    var cant = d.Cantidad;
                    var imp = d.Cantidad * d.PrecioUnitario;

                    if (acumuladores.TryGetValue(id, out var acc))
                        acumuladores[id] = (acc.cant + cant, acc.imp + imp);
                    else
                        acumuladores[id] = (cant, imp);
                    }
                }

            var puPromedio = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in acumuladores)
                {
                var (cant, imp) = kv.Value;
                puPromedio[kv.Key] = cant == 0m ? 0m : imp / cant;
                }

            foreach (var raiz in Arbol)
                ProcesarNodo(raiz);

            void ProcesarNodo(Nodo n)
                {
                if (n == null) return;

                if (n.Inferiores != null && n.Inferiores.Count > 0)
                    {
                    foreach (var h in n.Inferiores)
                        ProcesarNodo(h);

                    var sumaUnitReal = n.Inferiores.Sum(h => (h?.CantidadReal ?? 0m) * (h?.Pur1 ?? 0m));
                    n.Pur1 = sumaUnitReal;
                    }
                else
                    {
                    var key = ExtraerInsumoIdDesdeNodo(n.ID);
                    if (key != null && puPromedio.TryGetValue(key, out var pu))
                        n.Pur1 = pu;
                    else
                        n.Pur1 = 0m;
                    }
                }

            static string? ExtraerInsumoIdDesdeNodo(string idNodo)
                {
                if (string.IsNullOrWhiteSpace(idNodo)) return null;
                const string pref = "I-INP-";
                if (idNodo.StartsWith(pref, StringComparison.OrdinalIgnoreCase))
                    {
                    if (idNodo.Length > pref.Length + 2)
                        return idNodo.Substring(pref.Length + 2);
                    }
                return idNodo;
                }
            }

        // Ajustado: no toca Pur1, solo recalcula importes previstos y reales
        private void CalcularArbol(IEnumerable<Nodo> items, bool inicio, decimal factorSup)
            {
            int orden = 1;
            if (inicio)
                foreach (var n in items) n.OrdenInt = 0;

            foreach (var item in items)
                {
                item.OrdenInt = (item.OrdenInt == 0) ? orden++ : item.OrdenInt;

                if (item.HasItems)
                    {
                    if (item.Sup)
                        factorSup = item.Cantidad;

                    CalcularArbol(item.Inferiores, false, factorSup * item.Cantidad);

                    item.Factor = factorSup;

                    var importeHijos = item.Inferiores.Sum(x => x.Importe1);
                    item.PU1 = importeHijos;
                    item.Importe1 = item.Cantidad * item.PU1;

                    item.ImporteReal1 = item.CantidadReal * item.Pur1;
                    }
                else
                    {
                    item.Importe1 = item.Cantidad * item.PU1;
                    item.ImporteReal1 = item.CantidadReal * item.Pur1;
                    }

                if (!item.HasItems)
                    item.Factor = factorSup;
                }
            }

        private Dictionary<string, decimal> ObtenerIdsInsumosDeDetalles()
            {
            var resultado = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            if (_detallesRelacionados == null || _detallesRelacionados.Count == 0) return resultado;

            foreach (var d in _detallesRelacionados)
                {
                var id = d?.InsumoID?.Trim();
                if (string.IsNullOrWhiteSpace(id)) continue;
                if (resultado.TryGetValue(id, out var acum))
                    resultado[id] = acum + d.Cantidad;
                else
                    resultado[id] = d.Cantidad;
                }
            return resultado;
            }

        // AJUSTE: incluir tareas aunque su CantidadReal sea 0 para poder resetear descendencia.
        private static List<Nodo> SeleccionarNodosConInferioresEnIds(IEnumerable<Nodo> raiz, HashSet<string> idsInsumos, bool incluirDescendientes = true)
            {
            var resultado = new List<Nodo>();
            if (raiz == null || idsInsumos == null || idsInsumos.Count == 0) return resultado;

            foreach (var n in raiz)
                {
                if (n == null) continue;
                bool tieneInferiorEnIds = false;

                if (n.Inferiores != null && n.Inferiores.Count > 0)
                    {
                    if (incluirDescendientes)
                        {
                        tieneInferiorEnIds = n.Inferiores.Any(h =>
                            h != null && (
                                (!string.IsNullOrWhiteSpace(h.ID) && idsInsumos.Contains(h.ID)) ||
                                SubarbolContieneId(h, idsInsumos)
                            ));
                        }
                    else
                        {
                        tieneInferiorEnIds = n.Inferiores.Any(h =>
                            h != null && !string.IsNullOrWhiteSpace(h.ID) && idsInsumos.Contains(h.ID));
                        }
                    }

                // Incluye toda tarea con inferiores relevantes, aunque su CantidadReal sea 0.
                if (string.Equals(n.Tipo, "T", StringComparison.OrdinalIgnoreCase) && tieneInferiorEnIds)
                    resultado.Add(n);

                if (n.Inferiores != null && n.Inferiores.Count > 0)
                    resultado.AddRange(SeleccionarNodosConInferioresEnIds(n.Inferiores, idsInsumos, incluirDescendientes));
                }
            return resultado;
            }

        private static bool SubarbolContieneId(Nodo nodo, HashSet<string> idsInsumos)
            {
            if (nodo == null || nodo.Inferiores == null || nodo.Inferiores.Count == 0) return false;
            foreach (var h in nodo.Inferiores)
                {
                if (h == null) continue;
                if (!string.IsNullOrWhiteSpace(h.ID) && idsInsumos.Contains(h.ID))
                    return true;
                if (SubarbolContieneId(h, idsInsumos))
                    return true;
                }
            return false;
            }

        // NUEVO: redistribuye CantidadReal por InsumoID a nodos existentes ponderando por incidencia (Importe1 hijo / suma Importes del padre)
        // AJUSTE: si una tarea tiene CantidadReal == 0, toda su descendencia queda en 0 y no se distribuye en su rama.
        private void RedistribuirCantidadesPorInsumo(Dictionary<string, decimal> cantidadesPorInsumo, List<Nodo> nodosConGastos)
            {
            if (cantidadesPorInsumo == null || cantidadesPorInsumo.Count == 0) return;
            if (nodosConGastos == null || nodosConGastos.Count == 0) return;

            var idsAfectados = new HashSet<string>(cantidadesPorInsumo.Keys, StringComparer.OrdinalIgnoreCase);

            // A) Reset de descendencia para tareas con CantidadReal == 0
            foreach (var tarea in nodosConGastos.Where(t => string.Equals(t.Tipo, "T", StringComparison.OrdinalIgnoreCase) && t.CantidadReal == 0m))
                {
                foreach (var n in EnumerarArbol(new[] { tarea }))
                    {
                    if (!ReferenceEquals(n, tarea))
                        n.CantidadReal = 0m;
                    }
                }

            // B) Reset SOLO dentro de las ramas de nodosConGastos para IDs afectados
            foreach (var raiz in nodosConGastos)
                {
                foreach (var n in EnumerarArbol(new[] { raiz }))
                    {
                    if (!string.IsNullOrWhiteSpace(n.ID) && idsAfectados.Contains(n.ID))
                        n.CantidadReal = 0m;
                    }
                }

            var candidatosPorId = new Dictionary<string, List<(Nodo node, Nodo parent)>>(StringComparer.OrdinalIgnoreCase);
            var visitados = new HashSet<Nodo>();

            foreach (var raiz in nodosConGastos)
                {
                if (raiz?.Inferiores == null || raiz.Inferiores.Count == 0) continue;

                // Si el padre (tarea) está en 0, no distribuir en su rama (ya reseteada arriba).
                if (string.Equals(raiz.Tipo, "T", StringComparison.OrdinalIgnoreCase) && raiz.CantidadReal == 0m)
                    continue;

                foreach (var (node, parent) in EnumerarDescendenciaConPadre(raiz))
                    {
                    if (node == null) continue;
                    var id = node.ID;
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (!cantidadesPorInsumo.ContainsKey(id)) continue;
                    if (!visitados.Add(node)) continue;

                    if (!candidatosPorId.TryGetValue(id, out var lista))
                        {
                        lista = new List<(Nodo node, Nodo parent)>();
                        candidatosPorId[id] = lista;
                        }
                    lista.Add((node, parent));
                    }
                }

            // Garantizar que insumos sin candidatos sigan representados como "no imputados"
            foreach (var insumoId in cantidadesPorInsumo.Keys)
                {
                if (!candidatosPorId.ContainsKey(insumoId))
                    AddInsumoNoImputadoIfMissing(insumoId);
                }

            foreach (var kv in candidatosPorId)
                {
                var insumoId = kv.Key;
                if (!cantidadesPorInsumo.TryGetValue(insumoId, out var totalCantidad) || totalCantidad == 0m)
                    {
                    AddInsumoNoImputadoIfMissing(insumoId);
                    continue;
                    }

                var lista = kv.Value;
                if (lista == null || lista.Count == 0)
                    {
                    AddInsumoNoImputadoIfMissing(insumoId);
                    continue;
                    }

                var incidencias = new List<(Nodo node, decimal inc)>(lista.Count);
                foreach (var (node, parent) in lista)
                    {
                    decimal inc = CalcularIncidenciaRespectoAlPadre(node, parent);
                    incidencias.Add((node, inc));
                    }

                decimal sumaInc = incidencias.Sum(x => x.inc);

                if (sumaInc == 0m)
                    {
                    var sumaImportes = lista.Sum(x => x.node.Importe1);
                    if (sumaImportes > 0m)
                        {
                        incidencias = lista
                            .Select(x => (x.node, inc: (sumaImportes == 0m ? 0m : x.node.Importe1 / sumaImportes)))
                            .ToList();
                        sumaInc = 1m;
                        }
                    else
                        {
                        var eq = 1m / lista.Count;
                        incidencias = lista.Select(x => (x.node, inc: eq)).ToList();
                        sumaInc = 1m;
                        }
                    }

                bool algunaAsignacion = false;

                foreach (var (node, inc) in incidencias)
                    {
                    var proporcion = (sumaInc == 0m) ? 0m : (inc / sumaInc);

                    // Ajuste por CantidadReal del padre para distribuir dentro de la tarea actual
                    var parent = lista.First(p => p.node == node).parent;
                    var cantPadre = (parent?.CantidadReal ?? 1m);
                    if (cantPadre == 0m)
                        {
                        node.CantidadReal = 0m;
                        continue;
                        }

                    var nuevaCantidad = (totalCantidad * proporcion) / cantPadre;
                    node.CantidadReal = nuevaCantidad;
                    if (nuevaCantidad != 0m) algunaAsignacion = true;
                    }

                if (algunaAsignacion)
                    RemoveInsumoNoImputado(insumoId);
                else
                    AddInsumoNoImputadoIfMissing(insumoId);
                }

            // Local: recorrido completo del árbol
            static IEnumerable<Nodo> EnumerarArbol(IEnumerable<Nodo> raices)
                {
                if (raices == null) yield break;
                var stack = new Stack<Nodo>(raices.Where(r => r != null));
                while (stack.Count > 0)
                    {
                    var n = stack.Pop();
                    yield return n;
                    if (n.Inferiores != null && n.Inferiores.Count > 0)
                        {
                        foreach (var h in n.Inferiores)
                            if (h != null) stack.Push(h);
                        }
                    }
                }
            }

        // Elimina del árbol "R-INP" cualquier nodo con ID "I-INP-{nat}-{insumoId}" y limpia contenedores vacíos.
        private void RemoveInsumoNoImputado(string insumoId)
            {
            if (string.IsNullOrWhiteSpace(insumoId)) return;

            var rubroNoImputados = Arbol.FirstOrDefault(r =>
                r != null && string.Equals(r.ID, "R-INP", StringComparison.OrdinalIgnoreCase));

            if (rubroNoImputados == null || rubroNoImputados.Inferiores == null) return;

            var tareasVacias = new List<Nodo>();

            foreach (var t in rubroNoImputados.Inferiores.ToList())
                {
                if (t == null || t.Inferiores == null) continue;

                var hijosAEliminar = t.Inferiores
                    .Where(h => h != null
                                && !string.IsNullOrWhiteSpace(h.ID)
                                && h.ID.StartsWith("I-INP-", StringComparison.OrdinalIgnoreCase)
                                && h.ID.EndsWith(insumoId, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var h in hijosAEliminar)
                    t.Inferiores.Remove(h);

                if (t.Inferiores.Count == 0)
                    tareasVacias.Add(t);
                }

            foreach (var tv in tareasVacias)
                rubroNoImputados.Inferiores.Remove(tv);

            if (rubroNoImputados.Inferiores.Count == 0)
                Arbol.Remove(rubroNoImputados);
            }

        // Asegura que el insumo exista bajo "R-INP/T-INP-{nat}" si no está ya presente.
        private void AddInsumoNoImputadoIfMissing(string insumoId)
            {
            if (string.IsNullOrWhiteSpace(insumoId)) return;

            var detallesDelInsumo = _detallesRelacionados
                .Where(d => d != null && string.Equals(d.InsumoID?.Trim(), insumoId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (detallesDelInsumo.Count == 0) return;

            var rubroNoImputados = Arbol.FirstOrDefault(r => r != null && string.Equals(r.ID, "R-INP", StringComparison.OrdinalIgnoreCase));
            if (rubroNoImputados == null)
                {
                rubroNoImputados = new Nodo
                    {
                    ID = "R-INP",
                    Descripcion = "Insumos no imputados",
                    Tipo = "R",
                    Unidad = "Gl",
                    Cantidad = 1,
                    CantidadReal = 1,
                    Sup = true,
                    Inferiores = new ObservableCollection<Nodo>()
                    };
                Arbol.Add(rubroNoImputados);
                }

            foreach (var grupoNat in detallesDelInsumo.GroupBy(d => d.TipoID))
                {
                char nat = grupoNat.Key == '\0' ? 'O' : grupoNat.Key;
                string tareaId = $"T-INP-{nat}";

                var tareaINP = rubroNoImputados.Inferiores.FirstOrDefault(t => t != null && string.Equals(t.ID, tareaId, StringComparison.OrdinalIgnoreCase));
                if (tareaINP == null)
                    {
                    tareaINP = new Nodo
                        {
                        ID = tareaId,
                        Descripcion = $"{NaturalezaDescripcion(nat)} no imputados",
                        Tipo = "T",
                        Unidad = "Gl",
                        Cantidad = 1,
                        CantidadReal = 1,
                        Sup = false,
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    rubroNoImputados.Inferiores.Add(tareaINP);
                    }

                var yaExiste = tareaINP.Inferiores.Any(h =>
                    h != null
                    && !string.IsNullOrWhiteSpace(h.ID)
                    && h.ID.StartsWith("I-INP-", StringComparison.OrdinalIgnoreCase)
                    && h.ID.EndsWith(insumoId, StringComparison.OrdinalIgnoreCase));

                if (yaExiste) continue;

                var cantidadTotal = grupoNat.Sum(d => d.Cantidad);
                var puAcum = grupoNat.Sum(d => d.Cantidad * d.PrecioUnitario);
                var pu = cantidadTotal == 0m ? 0m : puAcum / cantidadTotal;

                var detalleEjemplo = grupoNat.FirstOrDefault();

                var nuevoInsumo = new Nodo
                    {
                    ID = $"I-INP-{nat}-{insumoId}",
                    Descripcion = detalleEjemplo?.Descrip ?? $"Insumo {detalleEjemplo?.ID}",
                    Tipo = nat.ToString(),
                    Unidad = detalleEjemplo?.Unidad ?? "Gl",
                    Cantidad = cantidadTotal,
                    CantidadReal = cantidadTotal,
                    Pur1 = pu,
                    Sup = false,
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                tareaINP.Inferiores.Add(nuevoInsumo);
                }
            }

        private static decimal CalcularIncidenciaRespectoAlPadre(Nodo node, Nodo parent)
            {
            if (node == null) return 0m;
            if (parent == null) return 1m;

            decimal totalPadre = 0m;
            if (parent.Inferiores != null && parent.Inferiores.Count > 0)
                totalPadre = parent.Inferiores.Sum(c => c?.Importe1 ?? 0m);

            if (totalPadre == 0m) return 0m;
            return node.Importe1 / totalPadre;
            }

        private static IEnumerable<(Nodo node, Nodo parent)> EnumerarDescendenciaConPadre(Nodo root)
            {
            if (root == null) yield break;
            if (root.Inferiores == null || root.Inferiores.Count == 0) yield break;

            var stack = new Stack<(Nodo node, Nodo parent)>();
            foreach (var h in root.Inferiores)
                if (h != null) stack.Push((h, root));

            while (stack.Count > 0)
                {
                var (n, p) = stack.Pop();
                yield return (n, p);

                if (n.Inferiores != null && n.Inferiores.Count > 0)
                    {
                    foreach (var c in n.Inferiores)
                        if (c != null) stack.Push((c, n));
                    }
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
                CantidadReal = 1,
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
                    CantidadReal = 1,
                    Sup = false,
                    Inferiores = new ObservableCollection<Nodo>()
                    };

                var hijos = g
                    .Where(d => d != null)
                    .OrderBy(d => d.Descrip)
                    .Select(d => new Nodo
                        {
                        ID = $"I-INP-{nat}-{(d.InsumoID ?? d.ID.ToString())}",
                        Descripcion = d.Descrip ?? $"Insumo {d.ID}",
                        Tipo = nat.ToString(),
                        Unidad = d.Unidad ?? "Gl",
                        Cantidad = d.Cantidad,
                        CantidadReal = d.Cantidad,
                        Pur1 = d.PrecioUnitario,
                        Sup = false,
                        Inferiores = new ObservableCollection<Nodo>()
                        });

                foreach (var h in hijos)
                    nodoT.Inferiores.Add(h);

                rubroNoImputados.Inferiores.Add(nodoT);
                }

            // Agregar SOLO UNA VEZ
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